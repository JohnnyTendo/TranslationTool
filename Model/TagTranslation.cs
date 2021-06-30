using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationTool.Model
{
    public class TagTranslation
    {
        int id;
        string tag;
        string text;
        bool isEdited;

        public int Id
        {
            get => id;
            set => id = value;
        }
        public string Tag
        {
            get => tag;
            set => tag = value;
        }
        public string Text
        {
            get => text;
            set => text = value;
        }
        public bool IsEdited
        {
            get => isEdited;
            set => isEdited = value;
        }
    }
}
