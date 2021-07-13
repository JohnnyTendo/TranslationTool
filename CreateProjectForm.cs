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
using TranslationTool.ViewModel;

namespace TranslationTool
{
    public partial class CreateProjectForm : Form
    {
        MainForm mainForm;
        TranslationViewModel ViewModel;
        public CreateProjectForm()
        {
            InitializeComponent();
            ViewModel = TranslationViewModel.Instance;
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
            FileConnector.getTranslationProject(fbd.SelectedPath);
            string modName = languageTextBox.Text + "Translation";
            ViewModel.project.Language = languageTextBox.Text;
            ViewModel.project.LanguageCode = languageCodeTextBox.Text;
            ViewModel.project.Mod = new ModFile()
            {
                author = authorTextBox.Text,
                blurb = blurbTextBox.Text,
                name = modName,
                url = "https://wildermyth.com/wiki/index.php?title=" + modName
            };

            this.Hide();
        }
    }
}
