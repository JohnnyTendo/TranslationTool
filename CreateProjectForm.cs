using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            mainForm.project.Language = languageTextBox.Text;
            mainForm.project.LanguageCode = languageCodeTextBox.Text;

            this.Hide();
        }
    }
}
