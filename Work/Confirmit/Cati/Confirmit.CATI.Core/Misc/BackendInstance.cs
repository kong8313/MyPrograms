using System.Web;

using Confirmit.CATI.Common.Exceptions;

namespace Confirmit.CATI.Core.Misc
{
    /// <summary>
    /// Class contains common properties for the current backend instance.
    /// </summary>
    public class BackendInstance
    {
        private static BackendInstance _backendInstance;

        private static BackendInstance GetCurrentBackendInstance()
        {
            if (_backendInstance != null)
            {
                return _backendInstance;
            }

            if (HttpContext.Current == null)
            {
                return null;
            }

            var backendInstanceFromHttpContext = (BackendInstance)HttpContext.Current.Items["BackendInstance"];

            return backendInstanceFromHttpContext;
        }

        private static void SetCurrentBackendInstance(BackendInstance current)
        {
            if (HttpContext.Current != null)
            {
                throw new InternalErrorException("Property BackendInstance.Current can be set only in the Backend service");
            }

            _backendInstance = current;
        }

        public static BackendInstance Current
        {
            get
            {
                var current = GetCurrentBackendInstance();

                if (current != null)
                {
                    return current;
                }

                if (HttpContext.Current != null)
                {
                    throw new InternalErrorException(
                        "HttpContext.Current.Items[\"BackendInstance\"] is not initialized");
                }

                throw new InternalErrorException("Property BackendInstance.Current is not initialized");
            }

            set
            {
                SetCurrentBackendInstance(value);
            }
        }

        /// <summary>
        /// Returns is BackendInstance.Current initialized.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                var current = GetCurrentBackendInstance();

                return current != null;
            }
        }

        /// <summary>
        /// Gets or sets company Id. 0 for default instance.
        /// </summary>
        [System.Obsolete("Deprecated. Use ICompanyInfo.CompanyId instead.")]
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets company name. "Default instance" for default instance.
        /// </summary>
        [System.Obsolete("Deprecated. Use ICompanyInfo.CompanyName instead.")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets company alias. "Default instance alias" for default instance.
        /// </summary>
        [System.Obsolete("Deprecated. Use ICompanyInfo.CompanyAlias instead.")]
        public string CompanyAlias { get; set; }

        [System.Obsolete("Deprecated. Use ICompanyAddonsInfo.HasCallCentersAddon instead.")]
        public bool HasCallCentersAddon { get; set; }

        /// <summary>
        /// Gets or sets the SQL connection string to the database.
        /// </summary>
        [System.Obsolete("Deprecated. Use IConnectionStrings.GetConnectionStringForSpecificCompany instead.")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets master connection string from config file
        /// </summary>
        [System.Obsolete("Deprecated. Use IConnectionStrings.MasterConnectionString instead.")]
        public string MasterConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the connection string to <c>confirmlog</c> database.
        /// </summary>
        [System.Obsolete("Deprecated. Use IConnectionStrings.ConfirmlogConnectionString instead.")]
        public string ConfirmlogConnectionString { get; set; }

        /// <summary>
        /// Gets or sets  the connection string to <c>confirm</c> database.
        /// </summary>
        [System.Obsolete("Deprecated. Use IConnectionStrings.ConfirmConnectionString instead.")]
        public string ConfirmConnectionString { get; set; }


        /// <summary>
        /// Gets or sets default connection string from config file
        /// </summary>
        [System.Obsolete("Deprecated. Use IConnectionStrings.DefaultInstanceConnectionString instead.")]
        public string DefaultInstanceConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current executing process is CATI backend instance service.
        /// </summary>
        [System.Obsolete("Deprecated. Use IInstanceInfo.IsExecutedInBackendInstance instead.")]
        public bool IsExecutedInBackendInstance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether current executing process is CATI backend default instance service.
        /// </summary>
        [System.Obsolete("Deprecated. Use IInstanceInfo.IsDefaultInstance instead.")]
        public bool IsDefaultInstance { get; set; }

        /// <summary>
        /// Gets a value indicating whether caching enabled.
        /// </summary>
        public bool IsCacheEnabled { get; set; }
    }
}