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
            TranslationProject output = _current;;
            foreach (TranslationDirectory dir in _import.Dirs)
            {
                string[] pathParts = dir.Path.Split('\\');

                TranslationDirectory currentDir = _current.Dirs.Find(d => d.Path.EndsWith(pathParts[pathParts.Length - 1]));
                if (currentDir == null)
                {
                    output.Dirs.Add(dir);
                }
                else
                {
                    output.Dirs.RemoveAll(d => d.Path.EndsWith(pathParts[pathParts.Length - 1]));
                    output.Dirs.Add(compareDirectories(currentDir, dir));
                }
            }
            return output;
        }

        private static TranslationDirectory compareDirectories(TranslationDirectory _current, TranslationDirectory _import)
        {
            TranslationDirectory output = _current;
            foreach (TranslationFile file in _import.Files)
            {
                string[] pathParts = file.Path.Split('\\');
                TranslationFile currentFile = _current.Files.Find(f => f.Path.EndsWith(pathParts[pathParts.Length - 1]));
                if (currentFile == null)
                {
                    output.Files.Add(file);
                }
                else
                {
                    output.Files.RemoveAll(f => f.Path.EndsWith(pathParts[pathParts.Length - 1]));
                    output.Files.Add(compareFiles(currentFile, file));
                }
            }
            foreach (TranslationDirectory dir in _import.Dirs)
            {
                string[] pathParts = dir.Path.Split('\\');
                TranslationDirectory currentDir = _current.Dirs.Find(d => d.Path.EndsWith(pathParts[pathParts.Length - 1])); 
                if (currentDir == null)
                {
                    output.Dirs.Add(dir);
                }
                else
                {
                    output.Dirs.RemoveAll(d => d.Path.EndsWith(pathParts[pathParts.Length - 1]));
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
            string context = _path.Remove(_path.LastIndexOf("\\"));
            _path = _path.Remove(0, _path.LastIndexOf("\\"));
            translationProject.Dirs.Add(getTranslationDir(context, _path, true));
            translationProject.Mod = readModFile(context + _path);
            TranslationViewModel.Instance.project = translationProject;
        }

        private static TranslationDirectory getTranslationDir(string _context, string _path, bool _firstRun = false)
        {
            // Rework this. This will cause the merge process to compare the creation context e.g. user folder as well.
            // Better create a complete context string and subtract it from the path

            TranslationDirectory translationDirectory;
            if (_firstRun)
            {
                translationDirectory = new TranslationDirectory() { Path = "Translation" };
                _path = "\\" + _path;
            }
            else
            {
                translationDirectory = new TranslationDirectory() { Path = _path };
            }

            DirectoryInfo rootDir = new DirectoryInfo(_context + _path);

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
                translationDirectory.Dirs.Add(getTranslationDir(_context + _path , "\\" + dir.Name));
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
