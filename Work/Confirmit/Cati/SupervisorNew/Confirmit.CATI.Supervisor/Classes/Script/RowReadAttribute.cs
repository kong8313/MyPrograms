using System;

namespace Confirmit.CATI.Supervisor.Classes.Script
{
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = false, Inherited = true )]
    public class RowReadAttribute : System.Attribute
    {
        private string m_ColumnKey;

        public RowReadAttribute( string columnKey )
        {
            m_ColumnKey = columnKey;
        }

        /// <summary>
        /// Infragistics grid Column Key
        /// </summary>
        public string ColumnKey
        {
            get { return m_ColumnKey; }
            set { m_ColumnKey = value; }
        }
    }

}