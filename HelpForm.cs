using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationTool.Texts;

namespace TranslationTool
{
    public partial class HelpForm : Form
    {
        int siteCounter = 0;
        List<RadioButton> radioButtons = new List<RadioButton>();
        public HelpForm()
        {
            InitializeComponent();
            radioButtons.Add(radioButton1);
            radioButtons.Add(radioButton2);
            radioButtons.Add(radioButton3);
            radioButtons.Add(radioButton4);
            radioButtons.Add(radioButton5);
            radioButtons.Add(radioButton6);
            radioButtons.Add(radioButton7);
            radioButtons.Add(radioButton8);
            titleLabel.Text = HelpTexts.helpPages[0].title;
            textLabel.Text = HelpTexts.helpPages[0].text;
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            changeSite(false);
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            changeSite(true);
        }

        private void changeSite(bool _moveNext)
        {
            int i;
            for (i = 0; i <= radioButtons.Count-1; i++)
            {
                if (_moveNext == true)
                {
                    if (radioButtons[i].Checked && i + 1 <= radioButtons.Count)
                    {
                        siteCounter = i + 1;
                        radioButtons[i].Checked = false;
                        radioButtons[siteCounter].Checked = true;
                        titleLabel.Text = HelpTexts.helpPages[siteCounter].title;
                        textLabel.Text = HelpTexts.helpPages[siteCounter].text;
                        break;
                    }
                }
                else
                {
                    if (radioButtons[i].Checked && i - 1 >= 0)
                    {
                        siteCounter = i - 1;
                        radioButtons[i].Checked = false;
                        radioButtons[siteCounter].Checked = true;
                        titleLabel.Text = HelpTexts.helpPages[siteCounter].title;
                        textLabel.Text = HelpTexts.helpPages[siteCounter].text;
                        break;
                    }
                }
            }
            if (siteCounter < radioButtons.Count-1)
            {
                nextButton.Enabled = true;
            }
            else
            {
                nextButton.Enabled = false;
            }
            if (siteCounter > 0)
            {
                previousButton.Enabled = true;
            }
            else
            {
                previousButton.Enabled = false;
            }
        }
    }
}
