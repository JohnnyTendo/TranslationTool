using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationTool.Model
{
    public class TranslationDirectory
    {
        List<TranslationFile> files = new List<TranslationFile>();
        List<TranslationDirectory> dirs = new List<TranslationDirectory>();
        string path;

        public List<TranslationFile> Files
        {
            get => files;
            set => files = value;
        }
        public List<TranslationDirectory> Dirs
        {
            get => dirs;
            set => dirs = value;
        }
        public string Path
        {
            get => path;
            set => path = value;
        }
    }
}
