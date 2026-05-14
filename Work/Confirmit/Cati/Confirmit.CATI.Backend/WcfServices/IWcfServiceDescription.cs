using System;

namespace Confirmit.CATI.Backend.WcfServices
{
    /// <summary>
    /// WCF service description.
    /// </summary>
    internal interface IWcfServiceDescription
    {
        /// <summary>
        /// Gets service name.
        /// Service name used in logging only so should be just user readable.
        /// </summary>
        string ServiceName
        {
            get;
        }

        /// <summary>
        /// Gets the service base Uri used to publish service.
        /// </summary>
        string Uri
        {
            get;
        }

        /// <summary>
        /// Gets type service is implemented in.
        /// </summary>
        Type ServiceType
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether service is external (called from internet) or internal (called from confirmit network only).
        /// </summary>
        bool IsExternal
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether service is must be secured with SSL(HTTPS) or not.
        /// </summary>
        bool RequireSchemaIndependentEndpointAddress
        {
            get;
        }
        
        /// <summary>
        /// Gets a value indicating whether service is must be published only on HTTP or not.
        /// </summary>
        bool IsInternalHttpOnly
        {
            get;
        }
    }
}
