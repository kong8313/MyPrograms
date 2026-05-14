using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ConsoleService.Abstract;

namespace Confirmit.CATI.Backend.WcfServices.External.ConsoleService
{
    /// <summary>
    /// Represents survey language. This class is wrapper for Confirmit 
    /// Language class.
    /// </summary>
    public class LanguageBuilder
    {
        /// <summary>
        /// Initializes new instance of Language class and fills it with given data.
        /// </summary>
        /// <param name="language">Confirmit language.</param>
        /// <exception cref="ArgumentNullException">language in null.</exception>
        public Language GetLanguage(Core.AuthoringService.Language language)
        {
            if (language == null)
            {
                throw new ArgumentNullException("language");
            }
            var lang = new Language();

            lang.ID = language.ID;
            lang.Name = language.Name;
            lang.LanguageIdentity = language.LanguageIdentity;
            lang.IsDefaultLanguage = language.IsDefaultLanguage;

            return lang;
        }

        /// <summary>
        /// Initializes new instance of LanguageCollection and fills it with specified data.
        /// </summary>
        /// <param name="languages">Collection of Confirmit languages.</param>
        public LanguageCollection GetLanguageCollection(IEnumerable<Core.AuthoringService.Language> languages)
        {
            if (languages == null)
            {
                throw new ArgumentNullException("languages");
            }

            var list = new LanguageCollection();
            list.AddRange(languages.Select(lang => GetLanguage(lang)));

            return list;
        }
    }
}
