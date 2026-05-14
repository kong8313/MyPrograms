using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Core.Services.CheckSpelling
{
    /// <summary>
    /// Provides service for SpellChecker component.
    /// </summary>
    public class CheckSpellingService
    {
        #region Members

        private Karamasoft.WebControls.UltimateSpell.Speller speller;

        #endregion

        #region Constructors

        public CheckSpellingService(int languageId)
        {
            string dictionaryPath = GetDictionaryPath(languageId);
            speller = new Karamasoft.WebControls.UltimateSpell.Speller(dictionaryPath) {IgnoreWordsInUpperCase = false};
        } 

        #endregion

        #region Methods

        /// <summary>
        /// Checks tesx spelling.
        /// </summary>
        /// <param name="textBlock">Text block.</param>
        /// <returns>Array of SpellError.</returns>
        public SpellError[] CheckText(string textBlock)
        {
            ArrayList errorList = speller.SpellCheck(textBlock);

            var result = (from Karamasoft.WebControls.UltimateSpell.SpellError error in errorList
                          select new SpellError(error.MisspelledWord,
                                                error.TextIndex,
                                                GetErrorType(error),
                                                (string[])error.Suggestions.ToArray(typeof(string))));

            return result.ToArray();
        }

        /// <summary>
        /// Returns absolute path to files of language dictionaries.
        /// </summary>
        ///<remarks>
        /// Absolute path is used because for some reason Karamasoft Speller doesn't undestand relative path.
        /// (Misspelled words are detected, but suggestions are not filled).
        ///</remarks>
        private string GetDictionaryPath(int languageId)
        {
            var cultureName = GetCultureName(languageId);
            var catiSystemDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);            
            var dictionaryPath = string.Format(@"UltimateSpellInclude\Dictionary\{0}\{0}.dic", cultureName);

            return Path.Combine(catiSystemDirectory, dictionaryPath);
        }

        private ErrorType GetErrorType(Karamasoft.WebControls.UltimateSpell.SpellError error)
        {
            switch (error.Type)
            {
                case Karamasoft.WebControls.UltimateSpell.SpellError.ErrorType.NotInDictionary:
                    return ErrorType.NotInDictionary;
                case Karamasoft.WebControls.UltimateSpell.SpellError.ErrorType.RepeatedWord:
                    return ErrorType.RepeatedWord;
                case Karamasoft.WebControls.UltimateSpell.SpellError.ErrorType.MixedCase:
                    return ErrorType.MixedCase;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Returns culture by language identifier.
        /// </summary>
        /// <param name="languageId">Language identifier.</param>
        private string GetCultureName(int languageId)
        {
            switch (languageId)
            {
                case 2:
                    return SupportedLanguage.Bulgarian.Description();
                case 3:
                    return SupportedLanguage.Catalan.Description();
                case 5:
                    return SupportedLanguage.Czech.Description();
                case 6:
                    return SupportedLanguage.Danish.Description();
                case 7:
                case 3079:
                case 5127:
                case 4103:
                case 1031:
                    return SupportedLanguage.German.Description();
                case 2055:
                    return SupportedLanguage.GermanSwitzerland.Description();
                case 10:
                case 16394:
                case 13322:
                case 9226:
                case 11274:
                case 5130:
                case 7178:
                case 12298:
                case 17418:
                case 4106:
                case 18442:
                case 3082:
                case 19466:
                case 6154:
                case 15370:
                case 10250:
                case 20490:
                case 1034:
                case 14346:
                case 8202:
                    return SupportedLanguage.Spanish.Description();
                case 2058:
                    return SupportedLanguage.SpanishMexico.Description();
                case 12:
                case 2060:
                case 3084:
                case 5132:
                case 6156:
                case 1036:
                case 4108:
                    return SupportedLanguage.French.Description();
                case 26:
                case 1050:
                case 3098:
                case 2074:
                    return SupportedLanguage.Croatian.Description();
                case 16:
                case 1040:
                case 2064:
                    return SupportedLanguage.Italian.Description();
                case 21:
                    return SupportedLanguage.Polish.Description();
                case 22:
                case 2070:
                    return SupportedLanguage.Portuguese.Description();
                case 1046:
                    return SupportedLanguage.PortugueseBrazil.Description();
                case 24:
                    return SupportedLanguage.Romanian.Description();
                case 25:
                    return SupportedLanguage.Russian.Description();
                case 29:
                    return SupportedLanguage.Swedish.Description();
                case 34:
                    return SupportedLanguage.Ukrainian.Description();
                case 36:
                    return SupportedLanguage.Slovenian.Description();
                case 39:
                case 2087:
                case 1063:
                    return SupportedLanguage.Lithuanian.Description();
                case 54:
                    return SupportedLanguage.AfricanSouthAfrica.Description();
                case 33:
                    return SupportedLanguage.Indonesian.Description();
                case 19:
                case 2067:
                case 1043:
                    return SupportedLanguage.DutchNetherlands.Description();
                case 20:
                case 1044:
                    return SupportedLanguage.NorwegianBokmal.Description();
                case 2068:
                    return SupportedLanguage.NorwegianNyNorsk.Description();
                case 1086:
                    return SupportedLanguage.Malay.Description();
                case 2057:
                    return SupportedLanguage.EnglishUK.Description();
                case 3081:
                    return SupportedLanguage.EngslishAustralia.Description();
                case 4105:
                    return SupportedLanguage.EnglishCanadian.Description();
                case 5129:
                    return SupportedLanguage.EnglishNewZeeland.Description();
                case 13:
                    return SupportedLanguage.Hebrew.Description();
                case 9:
                case 10249:
                case 9225:
                case 8201:
                case 13321:
                case 7177:
                case 11273:
                case 1033:
                case 12297:
                case 6153:
                    return SupportedLanguage.English.Description();
                default:
                    throw new SpellCheckerLanguageIsNotSupportedException(String.Format("Language '{0}' is not supported.", languageId));
            }
        }

        #endregion
    }
}
