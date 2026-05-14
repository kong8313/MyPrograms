using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using Confirmit.CATI.Supervisor.Core.Export;

namespace Confirmit.CATI.Supervisor.Core.Export.DataTableProvider
{
    public class DataTableExportRecordProvider : IExportRecordProvider
    {
        #region Fields

        private DataRow m_Row;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes new instance of DataTableExportRecordProvider class and fills it with given data.
        /// </summary>
        /// <param name="row">Data row.</param>
        public DataTableExportRecordProvider(DataRow row)
        {
            if(row == null)
            {
                throw new ArgumentNullException("row");
            }

            m_Row = row;
        }

        #endregion

        #region IExportRecordProvider Members

        /// <summary>
        /// Gets descendant records for this record.
        /// </summary>
        public IExportRecordProvider Descendants
        {
            get 
            {
                return null;
            }
        }


        /// <summary>
        /// Returns value of export item with given name.
        /// </summary>
        /// <param name="name">Export item name.</param>
        /// <returns>Export value.</returns>
        public object this[string name]
        {
            get 
            {
                object result = null;

                if (m_Row.Table.Columns.Contains(name))
                {
                    result = m_Row[name];
                }

                return result;
            }
        }

        #endregion

        #region IEnumerable<ExportItem> Members

        public IEnumerator<ExportItem> GetEnumerator()
        {
            foreach (DataColumn column in m_Row.Table.Columns)
            {
                yield return new ExportItem()
                {
                    Name = column.ColumnName,
                    Value = m_Row[column]
                };
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
