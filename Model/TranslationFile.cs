using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationTool.Model
{
    public class TranslationFile
    {
        List<TagTranslation> entries = new List<TagTranslation>();
        string path;

        public List<TagTranslation> Entries
        {
            get => entries;
            set => entries = value;
        }
        public string Path
        {
            get => path;
            set => path = value;
        }
    }
}
