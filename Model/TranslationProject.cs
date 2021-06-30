using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationTool.Model
{
    public class TranslationProject
    {
        List<TranslationFile> files = new List<TranslationFile>();
        string language;
        string languageCode;

        public List<TranslationFile> Files
        {
            get => files;
            set => files = value;
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
