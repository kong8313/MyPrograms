using System;

namespace Confirmit.CATI.Common.ConsoleService.Abstract
{
    /// <summary>
    /// Represents search parameter
    /// </summary>
    [Serializable]
    public class SearchParameter
    {
        public SearchParameter()
        {

        }

        /// <summary>
        /// Gets/sets name of column
        /// </summary>
        public string ColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets/sets type of search argument
        /// </summary>        
        public String ColumnTypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Search value
        /// </summary>
        public string Value
        {
            get;
            set;
        }        
    }
}
