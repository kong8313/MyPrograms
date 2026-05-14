using System.Collections.Generic;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Represents interview history entry. This class is wrapper for Confirmit 
    /// InterviewHistoryEntry class.
    /// </summary>
    public class QuestionHistory
    {
        #region Properties

        /// <summary>
        /// Gets/sets question identifier.
        /// </summary>
        public string QuestionId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets url query for interview history object.
        /// </summary>
        public string UrlQuery
        {
            get;
            set;
        }

        /// <summary>
        /// Localized question name.
        /// </summary>
        public string QuestionName
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// Represents the collection of QuestionHistory objects.
    /// </summary>
    public class QuestionHistoryCollection : List<QuestionHistory>
    {
    }
}