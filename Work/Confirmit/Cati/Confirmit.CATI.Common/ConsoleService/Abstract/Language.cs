using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    public class Language
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LanguageIdentity { get; set; }
        public bool IsDefaultLanguage { get; set; }
    }

    public class LanguageCollection : List<Language>
    {
        /// <summary>
        /// Returns first default language of collection.
        /// </summary>
        /// <returns>First default language if exists; otherwise null.</returns>
        public Language GetDefaultLanguage()
        {
            Language result = null;

            foreach (Language lang in this)
            {
                if (lang.IsDefaultLanguage)
                {
                    result = lang;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether an language with given identifier is in collection.
        /// </summary>
        /// <param name="languageId">Language identifier.</param>
        /// <returns>true, if language is found; otherwise false.</returns>
        public bool ContainsLanguage(int languageId)
        {
            return (IndexOf(languageId) != -1);
        }

        /// <summary>
        /// Searches for the language with specified identifier
        /// and returns the zero-based index of the first occurrence within the language list.
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns>The zero-based index of the first occurrence of language, if found; otherwise, –1.</returns>
        public int IndexOf(int languageId)
        {
            int result = -1;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].ID == languageId)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets language name by language identifier.
        /// If there is no language with such identifier empty string will be returned.
        /// </summary>
        /// <param name="languageId">Language identifier.</param>
        /// <returns>Language name.</returns>
        public string GetLanguageNameByLanguageId(int languageId)
        {
            string result = String.Empty;

            if (ContainsLanguage(languageId))
            {
                int index = IndexOf(languageId);
                result = this[index].Name;
            }

            return result;
        }
    }
}
