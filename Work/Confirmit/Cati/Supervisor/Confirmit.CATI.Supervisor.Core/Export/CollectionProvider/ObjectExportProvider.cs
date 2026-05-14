using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace Confirmit.CATI.Supervisor.Core.Export.CollectionProvider
{
    /// <summary>
    /// Represents export data provider for certain record.
    /// </summary>
    public class ObjectExportRecordProvider : IExportRecordProvider
    {
        #region Fields

        protected object m_Object;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes new instance of DataTableExportRecordProvider class and fills it with given data.
        /// </summary>
        /// <param name="row">Data row.</param>
        public ObjectExportRecordProvider(object obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            m_Object = obj;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns type of internal object
        /// </summary>
        private Type ObjectType
        {
            get
            {
                return m_Object.GetType();
            }
        }

        #endregion

        #region IExportRecordProvider Members

        /// <summary>
        /// Gets descendant records for this record.
        /// </summary>
        public virtual IExportRecordProvider Descendants
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
        public virtual object this[string name]
        {
            get 
            {
                object result = null;

                PropertyInfo property = ObjectType.GetProperties().Where(x => x.Name == name).FirstOrDefault();

                if (property != null)
                {
                    result = property.GetValue(m_Object, null);
                }
                
                return result;
            }
        }

        #endregion

        #region IEnumerable<ExportItem> Members

        public IEnumerator<ExportItem> GetEnumerator()
        {
            var properties = ObjectType.GetProperties();
            
            foreach (PropertyInfo pi in properties)
            {
                yield return new ExportItem()
                {
                    Name = pi.Name,
                    Value = pi.GetValue(m_Object, null)
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
