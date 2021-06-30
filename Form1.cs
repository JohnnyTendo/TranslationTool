using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationTool.Model;

namespace TranslationTool
{
    public partial class Form1 : Form
    {
        string fileName;
        string projectName;
        TagTranslation activeTag;
        public List<TagTranslation> tags = new List<TagTranslation>();
        public Form1()
        {
            InitializeComponent();
            fileDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            fileDataGrid.SelectionChanged += new EventHandler(OnSelectedRow);
            fileDataGrid.DataSource = tags;
            fileDataGrid.Columns[0].Width = 50;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Property files (*.properties)|*.properties|All (*.*)|*.*";
            ofd.ShowDialog();
            fileName = ofd.FileName;
            tags = FileConnector.getEntries(fileName);
            fileDataGrid.DataSource = tags;
            updateProgress();
        }

        private void OnSelectedRow(object sender, EventArgs e)
        {
            if (fileDataGrid.SelectedRows.Count <= 0)
            {
                return;
            }
            activeTag = ((TagTranslation)fileDataGrid.SelectedRows[0].DataBoundItem);
            nameTextBox.Text = activeTag.Tag;
            textTextBox.Text = activeTag.Text;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            TagTranslation tag = tags.Find(t => t.Id == activeTag.Id);
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

            projectName = sfd.FileName;
            FileConnector.writeJsonFile(sfd.FileName, tags);
        }

        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Project file (*.translation)|*.translation";
            ofd.ShowDialog();
            projectName = ofd.FileName;
            tags = FileConnector.readJsonFile(ofd.FileName);
            fileDataGrid.DataSource = tags;
            updateProgress();
        }

        private void finishProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Property files (*.properties)|*.properties|All (*.*)|*.*";
            sfd.ShowDialog();

            FileConnector.createTranslation(sfd.FileName, tags);
        }

        private void updateProgress()
        {
            progressBar.Maximum = tags.Count;
            progressBar.Value = tags.FindAll(t => t.IsEdited == true).Count;
            progressBar.Update();
            int percent = (int)(((double)progressBar.Value / (double)progressBar.Maximum) * 100);
            progressCurrentLabel.Text = percent + "%";
            progressBar.Refresh();
        }

        private void mergeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Project file (*.translation)|*.translation| All (*.*)|*.*";
            ofd.ShowDialog();
            List<TagTranslation> importedTags = FileConnector.readJsonFile(ofd.FileName);
            foreach (TagTranslation importedTag in importedTags.FindAll(t => t.IsEdited == true))
            {
                TagTranslation tag = tags.Find(t => t.Tag == importedTag.Tag && t.IsEdited == false);
                if (tag != null)
                {
                    tag.Text = importedTag.Text;
                    tag.IsEdited = true;
                }
            }
            fileDataGrid.DataSource = tags;
            updateProgress();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }
    }
}
