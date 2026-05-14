using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Supervisor.Core.Activity;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider.SpecificProvider
{   
    /// <summary>
    /// Represents export data provider for StatusInfo object.
    /// </summary>    
    public class StatusInfoExportProvider : IExportRecordProvider
    {
        #region Fields

        private List<StatusInfo> m_List = new List<StatusInfo>(); 

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes new instance of StatusInfoExportProvider class and fills it with given data.
        /// </summary>
        /// <param name="row">Data row.</param>
        public StatusInfoExportProvider(List<StatusInfo> list)
        {
            m_List = list;
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
        public object this[string name]
        {
            get
            {
                return m_List.Where(x => x.Name == name).FirstOrDefault();               
            }
        }

        #endregion

        #region IEnumerable<ExportItem> Members

        public IEnumerator<ExportItem> GetEnumerator()
        {
            foreach (StatusInfo si in m_List)
            {
                yield return new ExportItem() { Name = si.Name, Value = si.Value };
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
