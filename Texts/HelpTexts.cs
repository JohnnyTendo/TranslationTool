using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationTool.Model;

namespace TranslationTool.Texts
{
    public static class HelpTexts
    {
        public static List<HelpPageContent> helpPages = new List<HelpPageContent>()
        {
            new HelpPageContent()
            {
                title = "Welcome",
                text =  @"0"
            },
            new HelpPageContent()
            {
                title = "Project > New Project",
                text =  @"1"
            },
            new HelpPageContent()
            {
                title = "Project > Open Project",
                text =  @"2"
            },
            new HelpPageContent()
            {
                title = "Project > Save Project",
                text =  @"3"
            },
            new HelpPageContent()
            {
                title = "Project > Finish Project",
                text =  @"4"
            },
            new HelpPageContent()
            {
                title = "Merge File",
                text =  @"5"
            }
        };
    }
}
