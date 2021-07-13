using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TranslationTool.Model;
using TranslationTool.ViewModel;

namespace TranslationTool
{
    public partial class MainForm : Form
    {
        CreateProjectForm createForm = new CreateProjectForm();
        TranslationViewModel ViewModel;
        public MainForm()
        {
            InitializeComponent();
            ViewModel = TranslationViewModel.Instance;
            fileDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            fileDataGrid.SelectionChanged += new EventHandler(OnSelectedRow);
            fileDataGrid.DataSource = ViewModel.tags;
        }

        #region FormControl methods
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
            ViewModel.activeTag = ((TranslationTag)fileDataGrid.SelectedRows[0].DataBoundItem);
            nameTextBox.Text = ViewModel.activeTag.Tag;
            textTextBox.Text = ViewModel.activeTag.Text;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            TranslationTag tag = ViewModel.tags.Find(t => t.Id == ViewModel.activeTag.Id);
            tag.Text = textTextBox.Text;
            tag.IsEdited = true;
            int curRow = fileDataGrid.SelectedRows[0].Index;
            fileDataGrid.Rows[curRow].Selected = false;
            fileDataGrid.Rows[curRow+1].Selected = true;
            updateProgress();
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Project file (*.translation)|*.translation";
            sfd.ShowDialog();
            FileConnector.projectToJson(sfd.FileName);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Project file (*.translation)|*.translation";
            ofd.ShowDialog();
            FileConnector.projectFromJson(ofd.FileName);
            updateProgress();
            updateTreeView();
        }

        private void finishProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath == null || fbd.SelectedPath == "")
                return;
            FileConnector.createProject(fbd.SelectedPath);
        }

        private void mergeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Project file (*.translation)|*.translation| All (*.*)|*.*";
            ofd.ShowDialog();
            FileConnector.mergeJsonFile(ofd.FileName);
            updateTreeView();
            updateProgress();
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

        private void projectTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string tag = e.Node.Tag.ToString();
            List<TranslationFile> files = new List<TranslationFile>();
            if (tag.EndsWith(".properties"))
            {
                foreach (TranslationDirectory dir in ViewModel.project.Dirs)
                {
                    files = (List<TranslationFile>)dir.Files.FindAll(f => f.Path == tag);
                    if (files.Count > 0)
                    {
                        ViewModel.tags = files[0].Entries;
                        fileDataGrid.DataSource = ViewModel.tags;
                        updateProgress();
                        return;
                    }
                    files = searchTranslationDir(dir, tag);
                    if (files.Count > 0)
                    {
                        ViewModel.tags = files[0].Entries;
                        fileDataGrid.DataSource = ViewModel.tags;
                        updateProgress();
                        return;
                    }
                }
            }
            fileDataGrid.DataSource = ViewModel.tags;
            updateTreeView();
            updateProgress();
        }

        #endregion

        #region FormControl helper methods
        private void updateTreeView()
        {
            projectTreeView.BeginUpdate();
            projectTreeView.Nodes.Clear();
            TreeNode projectNode = new TreeNode() 
            { 
                Text = ViewModel.project.Language, 
                Tag = ViewModel.project 
            };
            foreach (TranslationDirectory dir in ViewModel.project.Dirs)
            {
                TreeNode node = readTranslationDirectory(dir);
                projectNode.Nodes.Add(node);
            }
            projectTreeView.Nodes.Add(projectNode);
            projectTreeView.EndUpdate();
            projectTreeView.Refresh();
            this.Refresh();
        }

        private void updateProgress()
        {
            createIndex();

            totalProgressBar.Maximum = ViewModel.index.Count;
            totalProgressBar.Value = ViewModel.index.FindAll(t => t.IsEdited == true).Count;
            totalProgressBar.Update();
            int totalPercent = (int)(((double)totalProgressBar.Value / (double)totalProgressBar.Maximum) * 100);
            if (totalPercent >= 0)
            {
                progressTotalLabel.Text = totalPercent + "%";
                totalProgressBar.Refresh();
            }

            currentProgressBar.Maximum = ViewModel.tags.Count;
            currentProgressBar.Value = ViewModel.tags.FindAll(t => t.IsEdited == true).Count;
            currentProgressBar.Update();
            int currentPercent = (int)(((double)currentProgressBar.Value / (double)currentProgressBar.Maximum) * 100);
            if (currentPercent >= 0)
            {
                progressCurrentLabel.Text = currentPercent + "%";
                currentProgressBar.Refresh();
            }
        }

        private void updateProgressBar(ProgressBar _progressBar, Label _progressBarLabel, List<TranslationTag> _payLoad)
        {
            _progressBar.Maximum = _payLoad.Count;
            _progressBar.Value = _payLoad.FindAll(t => t.IsEdited == true).Count;
            _progressBar.Update();
            int percent = (int)(((double)_progressBar.Value / (double)_progressBar.Maximum) * 100);
            if (percent > 0)
            {
                _progressBarLabel.Text = percent + "%";
                _progressBar.Refresh();
            }
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

        #endregion

        #region Helper methods
        private void createIndex()
        {
            foreach (TranslationDirectory dir in ViewModel.project.Dirs)
            {
                ViewModel.index.AddRange(readFileForIndex(dir));
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

        private static string trimPath(string _path)
        {
            string[] parts = _path.Split('\\');
            return parts[parts.Length - 1];
        }

        #endregion

    }
}
