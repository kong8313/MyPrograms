using System;
using System.Collections;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider
{
    /// <summary>
    /// Represents export data provider which takes data from IEnumarable object.
    /// </summary>
    public class CollectionExportProvider : IExportDataProvider
    {
        #region Fields

        protected IEnumerable m_Collection = null;
        protected IDictionary<string, string> m_AdditionalParams = new Dictionary<string, string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes new instance of CollectionExportProvider class and fills it with given data.
        /// </summary>        
        public CollectionExportProvider(IEnumerable collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            m_Collection = collection;        
        }

        /// <summary>
        /// Initializes new instance of CollectionExportProvider class and fills it with given data.
        /// </summary>        
        public CollectionExportProvider(IEnumerable collection, IDictionary<string, string> additionalParams)
            : this(collection)
        {
            if (additionalParams == null)
            {
                throw new ArgumentNullException("collection");
            }
            
            m_AdditionalParams = additionalParams;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns additional parameter by key
        /// </summary>
        /// <param name="key">Parameter key</param>
        /// <returns>Parameter value</returns>
        public string GetParameter(string key)
        {
            if (m_AdditionalParams.ContainsKey(key))
            {
                return m_AdditionalParams[key];
            }

            return String.Empty;
        }

        #endregion

        #region IEnumerable<IExportRecordProvider> Members

        public virtual IEnumerator<IExportRecordProvider> GetEnumerator()
        {
            foreach (object obj in m_Collection)
            {
                yield return new ObjectExportRecordProvider(obj);
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
