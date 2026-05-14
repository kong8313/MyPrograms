using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Exceptions
{
    /// <summary>
    /// Represents exception which occurs when you are trying to delete used filter.
    /// </summary>
    public class FilterIsUsedException : ApplicationException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterIsUsedException"/> class.
        /// </summary>
        public FilterIsUsedException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterIsUsedException"/> class.
        /// </summary>
        /// <param name="dependentFilterNames">The dependent filter names.</param>
        public FilterIsUsedException(IEnumerable<string> dependentFilterNames)
        {
            DependentFilterNames = new List<string>(dependentFilterNames);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the list of dependent filter names.
        /// </summary>
        public List<string> DependentFilterNames
        { 
            get;
            set;
        }
        #endregion
    }
}