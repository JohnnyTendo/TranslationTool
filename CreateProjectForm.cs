using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationTool.Model;

namespace TranslationTool
{
    public partial class CreateProjectForm : Form
    {
        MainForm mainForm;
        public CreateProjectForm()
        {
            InitializeComponent();
        }

        public void CreateNewProject(MainForm main)
        {
            mainForm = main;
            this.Show();
        }

        private void createProjectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            TranslationProject _project = FileConnector.getTranslationProject(fbd.SelectedPath); ;
            string modName = languageTextBox.Text + "Translation";
            _project.Language = languageTextBox.Text;
            _project.LanguageCode = languageCodeTextBox.Text;
            _project.Mod = new ModFile()
            {
                author = authorTextBox.Text,
                blurb = blurbTextBox.Text,
                name = modName,
                url = "https://wildermyth.com/wiki/index.php?title=" + modName
            };

            mainForm.project = _project;
            this.Hide();
        }
    }
}
