using System;
using System.Runtime.Serialization;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    [DataContract]
    public enum ErrorType
    {
        [EnumMember]
        NotInDictionary = 0,
        [EnumMember]
        MixedCase = 1,
        [EnumMember]
        RepeatedWord = 2,
    }

    /// <summary>
    /// Represents spell error.
    /// </summary>    
    [Serializable]
    public class SpellError
    {
        public  SpellError(string  misspelledWord, int misspelledWordIndex, ErrorType errorType, string[] suggestions )
        {
            if (misspelledWord == null)
            {
                throw new ArgumentNullException("misspelledWord");
            }
            if (suggestions == null)
            {
                throw new ArgumentNullException("suggestions");
            }

            MisspelledWord = misspelledWord;
            MisspelledWordIndex = misspelledWordIndex;
            ErrorType = errorType;
            Suggestions = suggestions;
        }

        public string MisspelledWord
        {
            get; 
            private set;
        }

        public int MisspelledWordIndex
        {
            get; 
            private set;
        }

        public ErrorType ErrorType
        {
            get;
            private set;
        }
        
        public string [] Suggestions
        {
            get; 
            private set;
        }        
    }
}
