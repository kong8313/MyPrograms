using System;

namespace Confirmit.CATI.Supervisor.Core.Export.Parse
{
    /// <summary>
    /// Represents lexeme type 
    /// </summary>
    public enum LexemeType
    {
        /// <summary>
        /// Means that 
        /// </summary>
        Data,
        Date,
        Resources,
        Params,
        HorizontalDetails
    }

    /// <summary>
    /// Represents information about lexeme
    /// </summary>
    public class LexemeInfo
    {
        /// <summary>
        /// Gets/sets lexeme type 
        /// </summary>
        public LexemeType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets lexeme value
        /// </summary>
        public string Value
        {
            get;
            set;
        }
    }
}
