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

            /*
             * If you are new to coding, please do not copy any of this style. This has nothing to do with
             * good coding and should be avoided. I simply don't want to add external files to the project.
             * Don't judge me for this one. 
             */

            new HelpPageContent()
            {
                title = "Welcome",
                text =  @"This is a brief help page to help you getting started translating Wildermyth using this tool. 
The following pages will contain an explenation of the menus of this tool.
Each of the following pages will take one menu item and function in its focus. The Pages contain the 
following:

  - GUI
  - New Project
  - Open Project
  - Save Project
  - Finish Project
  - Merge Files

If you encounter any errors, please contact me on Discord:
JohnnyTendo#0392"
            },
            new HelpPageContent()
            {
                title = "GUI (1/2)",
                text =  @"This page will explain the GUI or user interface to you.

The GUI consists of 4 parts:
  The current translation file (upper left), 
  the project tree (upper right),
  the texteditor (lower left)
  and the progress overview (lower right).


Current translation file
  Here you can see the representation of the selected file. Every line is an individual tag that has to be 
  translated. They consist of three parts: the tag (or identifier), a checkbox and the text. The checkbox 
  indicates wether a line already has been edited. The selected line will be shown in the texteditor.

Project tree
  The Project tree shows the structure of your translation project. The first node will show the language 
  code of the project. The second will be the name of the root folder of your finalized mod. 
  Every following node represents either a folder or a translation file. You can double click a file 
  to open it (Current translation file) or a folder to expand it in the project tree.
"
            },
            new HelpPageContent()
            {
                title = "GUI (2/2)",
                text =  @"Texteditor
  The Texteditor is your working area. Here you can edit the text of a selected line 
  (Current translation file) and save the new text. This will also mark the line as edited 
  to help you keep track of your progress.

Progress:
  Here you can track your progress. The current progress shows the progress for the opened file.
  The total progress shows your overall progress on every tag in every file in every folder 
  of the project.
"
            },
            new HelpPageContent()
            {
                title = "Project > New Project",
                text =  @"This menu item allows you to create a new translation project.

To start a new project, enter the desired language, language code (see specifications in the official 
Wildermyth Wiki), author and a little blurb (short description).

Next click the 'Create Project'-Button. A dialog will appear, where you have to select the root folder 
of a template-translation. Template-translations are basically copys of the Wildermyth folder structure 
with only the speech specific folders and files. 

After selecting the root folder, your translation project will get created."
            },
            new HelpPageContent()
            {
                title = "Project > Open Project",
                text =  @"This menu item allows you to open an existing and saved translation project.

Simply select the project file in the opening dialog."
            },
            new HelpPageContent()
            {
                title = "Project > Save Project",
                text =  @"This menu item allows you to save an open translation project to a file.

You can use this file to share your current progress with others or simply save your work."
            },
            new HelpPageContent()
            {
                title = "Project > Finish Project",
                text =  @"This menu item allows you to finish a translation mod from an open project.

This mod will contain the folder and properties files as required by Wildermyth and also a mod.json file 
with some basic information (See 'Create Project'). So actually the translation mod is ready for the 
SteamWorkshop."
            },
            new HelpPageContent()
            {
                title = "Merge File",
                text =  @"This menu item allows you to merge a second project file into 
your current project.

When merging two projects the current project will always have priority. Only tags edited in the second 
project but not in the current project will be imported and marked as edited. 
Imported tags, folders and files that do not exist in the current project are added."
            }
        };
    }
}
