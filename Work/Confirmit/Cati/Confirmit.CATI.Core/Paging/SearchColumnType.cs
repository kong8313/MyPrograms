using System;

namespace Confirmit.CATI.Core.Paging
{
    /// <summary>
    /// Represents enumeration of column types supported by our list search procedure.
    /// </summary>
    public enum SearchColumnType
    {
        /// <summary>
        /// Column is not searchable (default)
        /// </summary>
        None,

        /// <summary>
        /// Text column.
        /// </summary>
        Text,

        /// <summary>
        /// Date column.
        /// </summary>
        DateTime,

        /// <summary>
        /// Drop down list of values.
        /// </summary>
        DropDown,

        /// <summary>
        /// Drop down list of values.
        /// </summary>
        TextDropDown,

        /// <summary>
        /// Integer number.
        /// </summary>
        Number,
        
        /// <summary>
        /// Floating point number.
        /// </summary>
        Decimal,
        
        /// <summary>
        /// Time span.
        /// </summary>
        TimeSpan,

        /// <summary>
        /// Predefined date period like yesterday, last month, one year...
        /// </summary>
        PredefinedDatePeriod
    }
}
