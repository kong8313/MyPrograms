using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIServerSettings : IServerSettings 
    {
        private IServerSettings _inner;

        public StubIServerSettings()
        {
            _inner = null;
        }

        public IServerSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _AccessAllowedIPAddresses;
        public Func<string> AccessAllowedIPAddressesGet;
        public Action<string> AccessAllowedIPAddressesSetString;

        string IServerSettings.AccessAllowedIPAddresses
        {
            get
            {
                if (AccessAllowedIPAddressesGet != null)
                {
                    return AccessAllowedIPAddressesGet();
                } else if (_inner != null)
                {
                    return ((IServerSettings)_inner).AccessAllowedIPAddresses;
                }

                if (AccessAllowedIPAddressesSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _AccessAllowedIPAddresses;
                }

                return default(string);
            }

            set
            {
                if (AccessAllowedIPAddressesSetString != null)
                {
                    AccessAllowedIPAddressesSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((IServerSettings)_inner).AccessAllowedIPAddresses = value;
                    return;
                }

                if (AccessAllowedIPAddressesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _AccessAllowedIPAddresses = value;
                }

            }
        }

        private int _BackendMinThreadPoolSize;
        public Func<int> BackendMinThreadPoolSizeGet;
        public Action<int> BackendMinThreadPoolSizeSetInt32;

        int IServerSettings.BackendMinThreadPoolSize
        {
            get
            {
                if (BackendMinThreadPoolSizeGet != null)
                {
                    return BackendMinThreadPoolSizeGet();
                } else if (_inner != null)
                {
                    return ((IServerSettings)_inner).BackendMinThreadPoolSize;
                }

                if (BackendMinThreadPoolSizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BackendMinThreadPoolSize;
                }

                return default(int);
            }

            set
            {
                if (BackendMinThreadPoolSizeSetInt32 != null)
                {
                    BackendMinThreadPoolSizeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IServerSettings)_inner).BackendMinThreadPoolSize = value;
                    return;
                }

                if (BackendMinThreadPoolSizeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BackendMinThreadPoolSize = value;
                }

            }
        }

        private bool _CreateCompanyDatabasesFromBackup;
        public Func<bool> CreateCompanyDatabasesFromBackupGet;
        public Action<bool> CreateCompanyDatabasesFromBackupSetBoolean;

        bool IServerSettings.CreateCompanyDatabasesFromBackup
        {
            get
            {
                if (CreateCompanyDatabasesFromBackupGet != null)
                {
                    return CreateCompanyDatabasesFromBackupGet();
                } else if (_inner != null)
                {
                    return ((IServerSettings)_inner).CreateCompanyDatabasesFromBackup;
                }

                if (CreateCompanyDatabasesFromBackupSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CreateCompanyDatabasesFromBackup;
                }

                return default(bool);
            }

            set
            {
                if (CreateCompanyDatabasesFromBackupSetBoolean != null)
                {
                    CreateCompanyDatabasesFromBackupSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IServerSettings)_inner).CreateCompanyDatabasesFromBackup = value;
                    return;
                }

                if (CreateCompanyDatabasesFromBackupGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CreateCompanyDatabasesFromBackup = value;
                }

            }
        }

        private int _ServiceStartTimeout;
        public Func<int> ServiceStartTimeoutGet;
        public Action<int> ServiceStartTimeoutSetInt32;

        int IServerSettings.ServiceStartTimeout
        {
            get
            {
                if (ServiceStartTimeoutGet != null)
                {
                    return ServiceStartTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IServerSettings)_inner).ServiceStartTimeout;
                }

                if (ServiceStartTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ServiceStartTimeout;
                }

                return default(int);
            }

            set
            {
                if (ServiceStartTimeoutSetInt32 != null)
                {
                    ServiceStartTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IServerSettings)_inner).ServiceStartTimeout = value;
                    return;
                }

                if (ServiceStartTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ServiceStartTimeout = value;
                }

            }
        }

    }
}