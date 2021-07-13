using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationTool.Model;

namespace TranslationTool
{
    public static class FileConnector
    {
        //ToDo: implement a functionality to rename the TemplateDirectory from the path of the finalized project.
        /* Is:
         * e.g.: \\[SelectedModExportLocation]\\[TemplateDirectoryName]\\mod.json
         * e.g.: \\[SelectedModExportLocation]\\[TemplateDirectoryName]\\[AllOtherSubDirectories]
         * 
         * Should:
         * e.g.: \\[SelectedModExportLocation]\\[TranslationProject.Language]Translation\\mod.json
         * e.g.: \\[SelectedModExportLocation]\\[TranslationProject.Language]Translation\\[AllOtherSubDirectories]
         */


        #region These methods save/open the project files

        public static TranslationProject projectFromJson(string _fileName)
        {
            TranslationProject translationProject = new TranslationProject();
            if (_fileName == null || _fileName == "")
                return translationProject;
            using (StreamReader file = File.OpenText(_fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                translationProject = (TranslationProject)serializer.Deserialize(file, typeof(TranslationProject));
            }

            return translationProject;
        }

        public static void projectToJson(string fileName, TranslationProject _translationProject)
        {
            using (StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, _translationProject);
            }
        }

        #endregion

        #region These methods read the wyldermyth mod structure 

        public static TranslationProject getTranslationProject(string _path)
        {
            TranslationProject translationProject = new TranslationProject();
            translationProject.Dirs.Add(getTranslationDir(_path));
            translationProject.Mod = readModFile(_path);
            return translationProject;
        }

        private static TranslationDirectory getTranslationDir(string _path)
        {
            TranslationDirectory translationDirectory = new TranslationDirectory() { Path = trimPath(_path) };

            //Iterate through every sub folder
            DirectoryInfo rootDir = new DirectoryInfo(_path);
            foreach (DirectoryInfo dir in rootDir.GetDirectories())
            {
                translationDirectory.Dirs.Add(getTranslationDir(dir.FullName));
            }

            //Iterate through every file in directory
            foreach (FileInfo file in rootDir.GetFiles())
            {
                if (file.Name.EndsWith(".properties"))
                {
                    translationDirectory.Files.Add(getTranslationFile(file.FullName));
                }
            }

            return translationDirectory;
        }

        private static TranslationFile getTranslationFile(string fileName)
        {
            TranslationFile file = new TranslationFile() { Path = trimPath(fileName), Entries = readFile(fileName) };
            return file;
        }

        private static List<TranslationTag> readFile(string fileName)
        {
            int i = 0;
            List<TranslationTag> tags = new List<TranslationTag>();
            try
            {
                string[] lines = File.ReadAllLines(fileName);
                foreach (string line in lines)
                {
                    if (line == "" || line.StartsWith("#"))
                    {
                        continue;
                    }
                    Console.WriteLine(line);
                    string[] parts = line.Split('=');
                    TranslationTag tag = new TranslationTag()
                    {
                        Id = i,
                        Tag = parts[0],
                        Text = parts[1]
                    };
                    tags.Add(tag);
                    i++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: " + ex.Message);
            }
            return tags;
        }
        #endregion

        #region These methods write the wyldermyth mod structure

        public static void createProject(TranslationProject _translationProject, string _rootDir)
        {
            Directory.CreateDirectory(_rootDir);
            foreach (TranslationDirectory dir in _translationProject.Dirs)
            {
                createDirectory(dir, _rootDir);
            }
            writeModFile(_translationProject.Mod, _rootDir + removeContextFromPath(_translationProject.Dirs[0].Path));
        }

        private static void createDirectory(TranslationDirectory _translationDirectory, string _context)
        {
            string path = _context + removeContextFromPath(_translationDirectory.Path);
            Directory.CreateDirectory(path);
            foreach (TranslationDirectory dir in _translationDirectory.Dirs)
            {
                createDirectory(dir, path);
            }
            foreach (TranslationFile file in _translationDirectory.Files)
            {
                createFile(file, path);
            }
        }

        private static void createFile(TranslationFile _translationFile, string _context)
        {
            string path = _context + removeContextFromPath(_translationFile.Path);
            using (StreamWriter writer = File.CreateText(path))
            {
                foreach (TranslationTag tag in _translationFile.Entries)
                {
                    writer.WriteLine(tag.Tag + "=" + tag.Text);
                }
            }
        }

        #endregion

        #region These methods read/write the mod file for Steam
        private static ModFile readModFile(string fileName)
        {
            ModFile modFile = new ModFile();
            try
            {
                using (StreamReader file = File.OpenText(fileName + "\\mod.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    modFile = (ModFile)serializer.Deserialize(file, typeof(ModFile));
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Creating 'mod.json' file...");

                using (StreamWriter file = File.CreateText(fileName + "\\mod.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, modFile);
                }
            }
            return modFile;
        }

        private static void writeModFile(ModFile _modFile, string _rootDir)
        {
            string path = _rootDir + "\\mod.json";
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, _modFile);
            }
        }

        #endregion

        #region Helper Methods

        private static string trimPath(string _path)
        {
            string ret = "";
            string[] parts = _path.Split('\\');
            for (int i = 1; i < parts.Length; i++)
            {
                ret += '\\' + parts[i];
            }
            return ret;
        }

        private static string removeContextFromPath(string _path)
        {
            string[] parts = _path.Split('\\');
            return '\\' + parts[parts.Length-1];
        }

        #endregion
    }
}
