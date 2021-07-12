using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationTool.Model
{
    public class TranslationProject
    {
        List<TranslationDirectory> dirs = new List<TranslationDirectory>();
        ModFile mod;
        string language;
        string languageCode;

        public List<TranslationDirectory> Dirs
        {
            get => dirs;
            set => dirs = value;
        }
        public ModFile Mod
        {
            get => mod;
            set => mod = value;
        }
        public string Language
        {
            get => language;
            set => language = value;
        }
        public string LanguageCode
        {
            get => languageCode;
            set => languageCode = value;
        }
    }
}
