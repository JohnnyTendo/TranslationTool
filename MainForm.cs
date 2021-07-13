using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TranslationTool.Model;

namespace TranslationTool
{
    public partial class MainForm : Form
    {
        //string fileName;
        TranslationTag activeTag;
        List<TranslationTag> index = new List<TranslationTag>();
        CreateProjectForm createForm = new CreateProjectForm();

        public TranslationProject project = new TranslationProject();

        public List<TranslationTag> tags = new List<TranslationTag>();
        public MainForm()
        {
            InitializeComponent();
            fileDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            fileDataGrid.SelectionChanged += new EventHandler(OnSelectedRow);
            fileDataGrid.DataSource = tags;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnSelectedRow(object sender, EventArgs e)
        {
            if (fileDataGrid.SelectedRows.Count <= 0)
            {
                return;
            }
            activeTag = ((TranslationTag)fileDataGrid.SelectedRows[0].DataBoundItem);
            nameTextBox.Text = activeTag.Tag;
            textTextBox.Text = activeTag.Text;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            TranslationTag tag = tags.Find(t => t.Id == activeTag.Id);
            tag.Text = textTextBox.Text;
            tag.IsEdited = true;
            int curRow = fileDataGrid.SelectedRows[0].Index;
            fileDataGrid.Rows[curRow].Selected = false;
            fileDataGrid.Rows[curRow+1].Selected = true;
            updateProgress();
            updateTreeView();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Project file (*.translation)|*.translation";
            sfd.ShowDialog();
            FileConnector.projectToJson(sfd.FileName, project);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Project file (*.translation)|*.translation";
            ofd.ShowDialog();
            project = FileConnector.projectFromJson(ofd.FileName);
            updateProgress();
            updateTreeView();
        }

        private void finishProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath == null || fbd.SelectedPath == "")
                return;
            FileConnector.createProject(project, fbd.SelectedPath);
        }

        private void updateProgress()
        {
            createIndex();

            totalProgressBar.Maximum = index.Count;
            totalProgressBar.Value = index.FindAll(t => t.IsEdited == true).Count;
            totalProgressBar.Update();
            int percentTotal = (int)(((double)totalProgressBar.Value / (double)totalProgressBar.Maximum) * 100);
            if (percentTotal > 0)
            {
                progressTotalLabel.Text = percentTotal + "%";
                totalProgressBar.Refresh();
            }

            currentProgressBar.Maximum = tags.Count;
            currentProgressBar.Value = tags.FindAll(t => t.IsEdited == true).Count;
            currentProgressBar.Update();
            int percentCurrent = (int)(((double)currentProgressBar.Value / (double)currentProgressBar.Maximum) * 100);
            if (percentCurrent > 0)
            {
                progressCurrentLabel.Text = percentCurrent + "%";
                currentProgressBar.Refresh();
            }
        }

        private void mergeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            throw new NotImplementedException();
            /*
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Project file (*.translation)|*.translation| All (*.*)|*.*";
            ofd.ShowDialog();
            List<TranslationTag> importedTags = FileConnector.readJsonFile(ofd.FileName);
            foreach (TranslationTag importedTag in importedTags.FindAll(t => t.IsEdited == true))
            {
                TranslationTag tag = tags.Find(t => t.Tag == importedTag.Tag && t.IsEdited == false);
                if (tag != null)
                {
                    tag.Text = importedTag.Text;
                    tag.IsEdited = true;
                }
            }
            fileDataGrid.DataSource = tags;
            updateProgress();
            */
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createForm.CreateNewProject(this);
            updateTreeView();
        }

        private void updateTreeView()
        {
            projectTreeView.BeginUpdate();
            projectTreeView.Nodes.Clear();
            TreeNode projectNode = new TreeNode() { Text = project.Language, Tag = project };
            foreach (TranslationDirectory dir in project.Dirs)
            {
                projectNode.Nodes.Add(readTranslationDirectory(dir));
            }
            projectTreeView.Nodes.Add(projectNode);
            projectTreeView.EndUpdate();
            projectTreeView.Refresh();
            this.Refresh();
        }

        private TreeNode readTranslationDirectory(TranslationDirectory _translationDirectory)
        {
            TreeNode node = new TreeNode() { Text = trimPath(_translationDirectory.Path), Tag = _translationDirectory.Path };
            foreach (TranslationDirectory subDir in _translationDirectory.Dirs)
            {
                node.Nodes.Add(readTranslationDirectory(subDir));
            }
            foreach (TranslationFile file in _translationDirectory.Files)
            {
                node.Nodes.Add(readTranslationFile(file));
            }

            return node;
        }
        private TreeNode readTranslationFile(TranslationFile _translationFile)
        {
            TreeNode node = new TreeNode() { Text = trimPath(_translationFile.Path), Tag = _translationFile.Path };
            return node;
        }

        private void projectTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string tag = e.Node.Tag.ToString();
            List<TranslationFile> files = new List<TranslationFile>();
            if (tag.EndsWith(".properties"))
            {
                foreach (TranslationDirectory dir in project.Dirs)
                {
                    files = (List<TranslationFile>)dir.Files.FindAll(f => f.Path == tag);
                    if (files.Count > 0)
                    {
                        tags = files[0].Entries;
                        fileDataGrid.DataSource = tags;
                        updateTreeView();
                        updateProgress();
                        return;
                    }
                    files = searchTranslationDir(dir, tag);
                    if (files.Count > 0)
                    {
                        tags = files[0].Entries;
                        fileDataGrid.DataSource = tags;
                        updateTreeView();
                        updateProgress();
                        return;
                    }
                }
            }
            fileDataGrid.DataSource = tags;
            updateTreeView();
            updateProgress();
        }

        private List<TranslationFile> searchTranslationDir(TranslationDirectory _translationDirectory, string _tag)
        {
            List<TranslationFile> files = new List<TranslationFile>();
            files = (List<TranslationFile>)_translationDirectory.Files.FindAll(f => f.Path == _tag);
            if (files.Count > 0)
            {
                return files;
            }
            foreach (TranslationDirectory subDir in _translationDirectory.Dirs)
            {
                files = searchTranslationDir(subDir, _tag);
                if (files.Count > 0)
                {
                    return files;
                }
            }

            return files;
        }

        private void createIndex()
        {
            foreach (TranslationDirectory dir in project.Dirs)
            {
                index.AddRange(readFileForIndex(dir));
            }
        }

        private List<TranslationTag> readFileForIndex(TranslationDirectory _translationDirectory)
        {
            List<TranslationTag> tags = new List<TranslationTag>();

            foreach (TranslationDirectory dir in _translationDirectory.Dirs)
            {
                tags.AddRange(readFileForIndex(dir));
            }
            foreach (TranslationFile file in _translationDirectory.Files)
            {
                tags.AddRange(file.Entries);
            }

            return tags;
        }

        private static string trimPath(string _path)
        {
            string[] parts = _path.Split('\\');
            return parts[parts.Length - 1];
        }
    }
}
