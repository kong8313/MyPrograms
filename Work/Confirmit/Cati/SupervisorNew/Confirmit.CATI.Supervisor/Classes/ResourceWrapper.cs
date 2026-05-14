using System;
using System.Reflection;
using System.Resources;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Classes
{
    /// <summary>
    /// Simple ResourceManager wrapper.
    /// </summary>
    public class ResourceWrapper : IResourceWrapper
    {
        private ResourceManager m_RM = new ResourceManager(
            "Confirmit.CATI.Supervisor.Resources.Strings",
            Assembly.Load("Confirmit.CATI.Supervisor.Resources")
        );

        private static ResourceWrapper m_Instance = new ResourceWrapper();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceWrapper"/> class.
        /// </summary>
        private ResourceWrapper()
        {
        }

        /// <summary>
        /// Gets the instance of the <see cref="ResourceWrapper"/> class.
        /// </summary>
        public static ResourceWrapper Instance
        {
            get{ return( m_Instance ); }
        }

        /// <summary>
        /// Gets the resource string by string index
        /// </summary>
        public string this[ string sResItemName ]
        {
            get{ return( GetString( sResItemName ) ); }
        }
		
        /// <summary>
        /// Gets the resource string by string resource identifier
        /// </summary>
        /// <param name="sResItemName">Resource identifier</param>
        /// <returns>Resource string or sResItemName if resource is not found.</returns>
        public string GetString( string sResItemName )
        {
            string sRes = m_RM.GetString( sResItemName );
            return String.IsNullOrEmpty(sRes) ? sResItemName : sRes;
        }
    }
}