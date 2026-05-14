using System;

namespace Confirmit.CATI.Supervisor.Core.ConfigurationsApi
{
    [Serializable]
    public class LanguageModel : IComparable
    {
        public string Id;
        public string Name;
        public string CombinedId;

        public int CompareTo(object obj)
        {
            var languageModel = (LanguageModel)obj;
            return string.Compare(Name, languageModel.Name);
        }
    }

    public class LanguagesModel
    {
        public LanguageModel[] Items;
    }
}