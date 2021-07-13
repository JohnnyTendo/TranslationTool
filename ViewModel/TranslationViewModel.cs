using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslationTool.Model;

namespace TranslationTool.ViewModel
{
    public class TranslationViewModel
    {
        #region SingletonPattern

        private static TranslationViewModel instance = new TranslationViewModel();

        public static TranslationViewModel Instance
        {
            get
            {
                return instance;
            }
        }

        #endregion


        public TranslationTag activeTag;
        public List<TranslationTag> index = new List<TranslationTag>();

        public TranslationProject project = new TranslationProject();

        public List<TranslationTag> tags = new List<TranslationTag>();

    }
}
