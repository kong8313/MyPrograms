using System;

namespace Confirmit.CATI.Supervisor.Core.Export
{
    /// <summary>
    /// Class represent column property
    /// </summary>
    internal class ColumnProperty
    {
        /// <summary>
        /// Creates instance of ColumnProperty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="style"></param>
        public ColumnProperty(string value, string style)
        {
            Value = value;
            Style = style;        
        }

        /// <summary>
        ///Gets/sets column value
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        ///Gets/sets column style
        /// </summary>
        public string Style
        {
            get;
            set;
        }
    }
}
