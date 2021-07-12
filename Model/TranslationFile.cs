using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationTool.Model
{
    public class TranslationFile
    {
        List<TranslationTag> entries = new List<TranslationTag>();
        string path;

        public List<TranslationTag> Entries
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
