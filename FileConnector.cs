using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationTool.Model;
using TranslationTool.ViewModel;

namespace TranslationTool
{
    public static class FileConnector
    {

        #region These methods save/open the project files

        public static void projectFromJson(string _fileName)
        {
            TranslationProject translationProject = new TranslationProject();
            if (_fileName == null || _fileName == "")
            { 
                TranslationViewModel.Instance.project = translationProject;
                return;
            }
            using (StreamReader file = File.OpenText(_fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                translationProject = (TranslationProject)serializer.Deserialize(file, typeof(TranslationProject));
            }

            TranslationViewModel.Instance.project = translationProject;
        }

        public static void projectToJson(string _fileName)
        {
            using (StreamWriter file = File.CreateText(_fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, TranslationViewModel.Instance.project);
            }
        }

        public static void mergeJsonFile(string _fileName)
        {
            TranslationViewModel ViewModel = TranslationViewModel.Instance;
            if (_fileName == null || _fileName == "")
            {
                return;
            }
            TranslationProject importProject = new TranslationProject();
            TranslationProject currentProject = ViewModel.project;
            List<TranslationDirectory> currentDirs = new List<TranslationDirectory>();

            using (StreamReader file = File.OpenText(_fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                importProject = (TranslationProject)serializer.Deserialize(file, typeof(TranslationProject));
            }
            ViewModel.project = compareProjects(currentProject, importProject);
        }

        private static TranslationProject compareProjects(TranslationProject _current, TranslationProject _import)
        {
            TranslationProject output = new TranslationProject() 
            { 
                Language = _current.Language, 
                LanguageCode = _current.LanguageCode, 
                Mod = _current.Mod 
            };
            foreach (TranslationDirectory dir in _import.Dirs)
            {
                TranslationDirectory currentDir = _current.Dirs.Find(d => d.Path == dir.Path);
                if (currentDir == null)
                {
                    output.Dirs.Add(dir);
                }
                else
                {
                    output.Dirs.Add(compareDirectories(currentDir, dir));
                }
            }
            return output;
        }

        private static TranslationDirectory compareDirectories(TranslationDirectory _current, TranslationDirectory _import)
        {
            TranslationDirectory output = new TranslationDirectory() { Path = _current.Path };
            foreach (TranslationFile file in _import.Files)
            {
                TranslationFile currentFile = _current.Files.Find(f => f.Path == file.Path);
                if (currentFile == null)
                {
                    output.Files.Add(file);
                }
                else
                {
                    output.Files.Add(compareFiles(currentFile, file));
                }
            }
            foreach (TranslationDirectory dir in _import.Dirs)
            {
                TranslationDirectory currentDir = _current.Dirs.Find(d => d.Path == dir.Path); 
                if (currentDir == null)
                {
                    output.Dirs.Add(dir);
                }
                else
                {
                    output.Dirs.Add(compareDirectories(currentDir, dir));
                }
            }
            return output;
        }

        private static TranslationFile compareFiles(TranslationFile _current, TranslationFile _import)
        {
            TranslationFile output = new TranslationFile() { Path = _current.Path };
            foreach (TranslationTag tag in _import.Entries)
            {
                TranslationTag currentTag = _current.Entries.Find(t => t.Tag == tag.Tag);
                if (currentTag == null)
                {
                    output.Entries.Add(tag);
                }
                else
                {
                    if (currentTag.IsEdited)
                        output.Entries.Add(currentTag);
                    else if (tag.IsEdited)
                        output.Entries.Add(tag);
                    else
                        output.Entries.Add(currentTag);
                }
            }
            return output;
        }


        #endregion

        #region These methods read the wyldermyth mod structure 

        public static void getTranslationProject(string _path)
        {
            TranslationProject translationProject = new TranslationProject();
            translationProject.Dirs.Add(getTranslationDir(_path, true));
            translationProject.Mod = readModFile(_path);
            TranslationViewModel.Instance.project = translationProject;
        }

        private static TranslationDirectory getTranslationDir(string _path, bool _firstRun = false)
        {
            TranslationDirectory translationDirectory;
            if (_firstRun)
            {
                string[] parts = _path.Split('\\');
                string modifiedPath = "";
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    modifiedPath += parts[i] + '\\';
                }
                modifiedPath += "Translation";
                translationDirectory = new TranslationDirectory() { Path = trimPath(modifiedPath) };
            }
            else
            {
                translationDirectory = new TranslationDirectory() { Path = trimPath(_path) };
            }

            DirectoryInfo rootDir = new DirectoryInfo(_path);

            //Iterate through every file in directory
            foreach (FileInfo file in rootDir.GetFiles())
            {
                if (file.Name.EndsWith(".properties"))
                {
                    translationDirectory.Files.Add(getTranslationFile(file.FullName));
                }
            }

            //Iterate through every sub folder
            foreach (DirectoryInfo dir in rootDir.GetDirectories())
            {
                translationDirectory.Dirs.Add(getTranslationDir(dir.FullName));
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

        public static void createProject(string _rootDir)
        {
            TranslationProject translationProject = TranslationViewModel.Instance.project;
            Directory.CreateDirectory(_rootDir);
            foreach (TranslationDirectory dir in translationProject.Dirs)
            {
                createDirectory(dir, _rootDir);
            }
            writeModFile(_rootDir + removeContextFromPath(translationProject.Dirs[0].Path));
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
            string languageInsert = "_" + TranslationViewModel.Instance.project.LanguageCode;
            path = path.Insert(path.LastIndexOf('.'), languageInsert);
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

        private static void writeModFile(string _rootDir)
        {
            ModFile modFile = TranslationViewModel.Instance.project.Mod;
            string path = _rootDir + "\\mod.json";
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, modFile);
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
