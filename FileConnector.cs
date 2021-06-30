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
        public static List<TagTranslation> getEntries(string fileName)
        {
            List<TagTranslation> tags = readFile(fileName);
            return tags;
        }
        public static void createTranslation(string fileName, List<TagTranslation> data)
        {
            using (StreamWriter writer = File.CreateText(fileName))
            {
                foreach (TagTranslation tag in data)
                {
                    writer.WriteLine(tag.Tag + "=" + tag.Text);
                }
            } 
        }

        private static List<TagTranslation> readFile(string fileName)
        {
            int i = 0;
            List<TagTranslation> tags = new List<TagTranslation>();
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
                    TagTranslation tag = new TagTranslation() 
                    { 
                        Id = i,
                        Tag = parts[0],
                        Text = parts[1]
                    };
                    tags.Add(tag);
                    i++;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error occured: " + ex.Message);
            }
            return tags;
        }

        public static List<TagTranslation> readJsonFile(string fileName)
        {
            List<TagTranslation> tags = new List<TagTranslation>();
            using (StreamReader file = File.OpenText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                tags = (List<TagTranslation>)serializer.Deserialize(file, typeof(List<TagTranslation>));
            }
            return tags;
        }

        public static void writeJsonFile(string fileName, List<TagTranslation> data)
        {
            using (StreamWriter file = File.CreateText(fileName))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, data);
            }
        }
    }
}
