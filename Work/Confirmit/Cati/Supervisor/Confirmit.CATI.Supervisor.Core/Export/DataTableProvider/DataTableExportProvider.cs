using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Confirmit.CATI.Supervisor.Core.Export;

namespace Confirmit.CATI.Supervisor.Core.Export.DataTableProvider
{
    /// <summary>
    /// Represents export data provider which takes data from DataTable object.
    /// </summary>
    public class DataTableExportProvider : IExportDataProvider
    {
        #region Fields

        private DataTable m_Table = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes new instance of DataTableExportProvider class and fills it with given data.
        /// </summary>
        /// <param name="table">Table</param>
        public DataTableExportProvider(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            m_Table = table;
        }

        #endregion

        #region IEnumerable<IExportRecordProvider> Members

        public IEnumerator<IExportRecordProvider> GetEnumerator()
        {
            foreach (DataRow row in m_Table.Rows)
            {
                yield return new DataTableExportRecordProvider(row);
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IExportDataProvider Members

        public string GetParameter(string key)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
