using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Paging
{
    /// <summary>
    /// Represents sorting arguments (property name and sorting direction) for single property.
    /// </summary>
    public class SortingArgs
    {
        /// <summary>
        /// Gets name for the property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets sorting direction for the property.
        /// </summary>
        public bool IsAscending { get; private set; }

        /// <summary>
        /// Creates sorting arguments object for CommonMultiComparer.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        /// <param name="isAscending">Sorting order.</param>
        public SortingArgs(string propertyName, bool isAscending)
        {
            PropertyName = propertyName;
            IsAscending = isAscending;
        }
    }

    /// <summary>
    /// Represents collection of SortingArgs elements.
    /// </summary>
    public class SortingArgsCollection : List<SortingArgs>
    {
    }
}
