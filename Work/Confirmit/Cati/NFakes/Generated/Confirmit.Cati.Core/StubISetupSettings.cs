using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISetupSettings : ISetupSettings 
    {
        private ISetupSettings _inner;

        public StubISetupSettings()
        {
            _inner = null;
        }

        public ISetupSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _BackendVersion;
        public Func<string> BackendVersionGet;
        public Action<string> BackendVersionSetString;

        string ISetupSettings.BackendVersion
        {
            get
            {
                if (BackendVersionGet != null)
                {
                    return BackendVersionGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).BackendVersion;
                }

                if (BackendVersionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BackendVersion;
                }

                return default(string);
            }

            set
            {
                if (BackendVersionSetString != null)
                {
                    BackendVersionSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).BackendVersion = value;
                    return;
                }

                if (BackendVersionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BackendVersion = value;
                }

            }
        }

        private string _BBCCVersion;
        public Func<string> BBCCVersionGet;
        public Action<string> BBCCVersionSetString;

        string ISetupSettings.BBCCVersion
        {
            get
            {
                if (BBCCVersionGet != null)
                {
                    return BBCCVersionGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).BBCCVersion;
                }

                if (BBCCVersionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BBCCVersion;
                }

                return default(string);
            }

            set
            {
                if (BBCCVersionSetString != null)
                {
                    BBCCVersionSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).BBCCVersion = value;
                    return;
                }

                if (BBCCVersionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BBCCVersion = value;
                }

            }
        }

        private string _CertificatePath;
        public Func<string> CertificatePathGet;
        public Action<string> CertificatePathSetString;

        string ISetupSettings.CertificatePath
        {
            get
            {
                if (CertificatePathGet != null)
                {
                    return CertificatePathGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).CertificatePath;
                }

                if (CertificatePathSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CertificatePath;
                }

                return default(string);
            }

            set
            {
                if (CertificatePathSetString != null)
                {
                    CertificatePathSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).CertificatePath = value;
                    return;
                }

                if (CertificatePathGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CertificatePath = value;
                }

            }
        }

        private string _CertificateType;
        public Func<string> CertificateTypeGet;
        public Action<string> CertificateTypeSetString;

        string ISetupSettings.CertificateType
        {
            get
            {
                if (CertificateTypeGet != null)
                {
                    return CertificateTypeGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).CertificateType;
                }

                if (CertificateTypeSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _CertificateType;
                }

                return default(string);
            }

            set
            {
                if (CertificateTypeSetString != null)
                {
                    CertificateTypeSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).CertificateType = value;
                    return;
                }

                if (CertificateTypeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _CertificateType = value;
                }

            }
        }

        private string _ConfirmitLinkedServerName;
        public Func<string> ConfirmitLinkedServerNameGet;
        public Action<string> ConfirmitLinkedServerNameSetString;

        string ISetupSettings.ConfirmitLinkedServerName
        {
            get
            {
                if (ConfirmitLinkedServerNameGet != null)
                {
                    return ConfirmitLinkedServerNameGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).ConfirmitLinkedServerName;
                }

                if (ConfirmitLinkedServerNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ConfirmitLinkedServerName;
                }

                return default(string);
            }

            set
            {
                if (ConfirmitLinkedServerNameSetString != null)
                {
                    ConfirmitLinkedServerNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).ConfirmitLinkedServerName = value;
                    return;
                }

                if (ConfirmitLinkedServerNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ConfirmitLinkedServerName = value;
                }

            }
        }

        private string _EncryptedCertificatePassword;
        public Func<string> EncryptedCertificatePasswordGet;
        public Action<string> EncryptedCertificatePasswordSetString;

        string ISetupSettings.EncryptedCertificatePassword
        {
            get
            {
                if (EncryptedCertificatePasswordGet != null)
                {
                    return EncryptedCertificatePasswordGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).EncryptedCertificatePassword;
                }

                if (EncryptedCertificatePasswordSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EncryptedCertificatePassword;
                }

                return default(string);
            }

            set
            {
                if (EncryptedCertificatePasswordSetString != null)
                {
                    EncryptedCertificatePasswordSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).EncryptedCertificatePassword = value;
                    return;
                }

                if (EncryptedCertificatePasswordGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EncryptedCertificatePassword = value;
                }

            }
        }

        private string _EncryptedConfirmConnectionString;
        public Func<string> EncryptedConfirmConnectionStringGet;
        public Action<string> EncryptedConfirmConnectionStringSetString;

        string ISetupSettings.EncryptedConfirmConnectionString
        {
            get
            {
                if (EncryptedConfirmConnectionStringGet != null)
                {
                    return EncryptedConfirmConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).EncryptedConfirmConnectionString;
                }

                if (EncryptedConfirmConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EncryptedConfirmConnectionString;
                }

                return default(string);
            }

            set
            {
                if (EncryptedConfirmConnectionStringSetString != null)
                {
                    EncryptedConfirmConnectionStringSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).EncryptedConfirmConnectionString = value;
                    return;
                }

                if (EncryptedConfirmConnectionStringGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EncryptedConfirmConnectionString = value;
                }

            }
        }

        private string _EncryptedConfirmlogConnectionString;
        public Func<string> EncryptedConfirmlogConnectionStringGet;
        public Action<string> EncryptedConfirmlogConnectionStringSetString;

        string ISetupSettings.EncryptedConfirmlogConnectionString
        {
            get
            {
                if (EncryptedConfirmlogConnectionStringGet != null)
                {
                    return EncryptedConfirmlogConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).EncryptedConfirmlogConnectionString;
                }

                if (EncryptedConfirmlogConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EncryptedConfirmlogConnectionString;
                }

                return default(string);
            }

            set
            {
                if (EncryptedConfirmlogConnectionStringSetString != null)
                {
                    EncryptedConfirmlogConnectionStringSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).EncryptedConfirmlogConnectionString = value;
                    return;
                }

                if (EncryptedConfirmlogConnectionStringGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EncryptedConfirmlogConnectionString = value;
                }

            }
        }

        private string _EncryptedSessionStateConnectionString;
        public Func<string> EncryptedSessionStateConnectionStringGet;
        public Action<string> EncryptedSessionStateConnectionStringSetString;

        string ISetupSettings.EncryptedSessionStateConnectionString
        {
            get
            {
                if (EncryptedSessionStateConnectionStringGet != null)
                {
                    return EncryptedSessionStateConnectionStringGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).EncryptedSessionStateConnectionString;
                }

                if (EncryptedSessionStateConnectionStringSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EncryptedSessionStateConnectionString;
                }

                return default(string);
            }

            set
            {
                if (EncryptedSessionStateConnectionStringSetString != null)
                {
                    EncryptedSessionStateConnectionStringSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).EncryptedSessionStateConnectionString = value;
                    return;
                }

                if (EncryptedSessionStateConnectionStringGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EncryptedSessionStateConnectionString = value;
                }

            }
        }

        private string _InstallLocation;
        public Func<string> InstallLocationGet;
        public Action<string> InstallLocationSetString;

        string ISetupSettings.InstallLocation
        {
            get
            {
                if (InstallLocationGet != null)
                {
                    return InstallLocationGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).InstallLocation;
                }

                if (InstallLocationSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InstallLocation;
                }

                return default(string);
            }

            set
            {
                if (InstallLocationSetString != null)
                {
                    InstallLocationSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).InstallLocation = value;
                    return;
                }

                if (InstallLocationGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InstallLocation = value;
                }

            }
        }

        private string _InterviewerAPIVersion;
        public Func<string> InterviewerAPIVersionGet;
        public Action<string> InterviewerAPIVersionSetString;

        string ISetupSettings.InterviewerAPIVersion
        {
            get
            {
                if (InterviewerAPIVersionGet != null)
                {
                    return InterviewerAPIVersionGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).InterviewerAPIVersion;
                }

                if (InterviewerAPIVersionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerAPIVersion;
                }

                return default(string);
            }

            set
            {
                if (InterviewerAPIVersionSetString != null)
                {
                    InterviewerAPIVersionSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).InterviewerAPIVersion = value;
                    return;
                }

                if (InterviewerAPIVersionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerAPIVersion = value;
                }

            }
        }

        private string _InterviewerConsoleVersion;
        public Func<string> InterviewerConsoleVersionGet;
        public Action<string> InterviewerConsoleVersionSetString;

        string ISetupSettings.InterviewerConsoleVersion
        {
            get
            {
                if (InterviewerConsoleVersionGet != null)
                {
                    return InterviewerConsoleVersionGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).InterviewerConsoleVersion;
                }

                if (InterviewerConsoleVersionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _InterviewerConsoleVersion;
                }

                return default(string);
            }

            set
            {
                if (InterviewerConsoleVersionSetString != null)
                {
                    InterviewerConsoleVersionSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).InterviewerConsoleVersion = value;
                    return;
                }

                if (InterviewerConsoleVersionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _InterviewerConsoleVersion = value;
                }

            }
        }

        private string _IsLoadBalancedEnvironment;
        public Func<string> IsLoadBalancedEnvironmentGet;
        public Action<string> IsLoadBalancedEnvironmentSetString;

        string ISetupSettings.IsLoadBalancedEnvironment
        {
            get
            {
                if (IsLoadBalancedEnvironmentGet != null)
                {
                    return IsLoadBalancedEnvironmentGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).IsLoadBalancedEnvironment;
                }

                if (IsLoadBalancedEnvironmentSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _IsLoadBalancedEnvironment;
                }

                return default(string);
            }

            set
            {
                if (IsLoadBalancedEnvironmentSetString != null)
                {
                    IsLoadBalancedEnvironmentSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).IsLoadBalancedEnvironment = value;
                    return;
                }

                if (IsLoadBalancedEnvironmentGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _IsLoadBalancedEnvironment = value;
                }

            }
        }

        private string _LoadBalancerIsAlivePageRenameTimeout;
        public Func<string> LoadBalancerIsAlivePageRenameTimeoutGet;
        public Action<string> LoadBalancerIsAlivePageRenameTimeoutSetString;

        string ISetupSettings.LoadBalancerIsAlivePageRenameTimeout
        {
            get
            {
                if (LoadBalancerIsAlivePageRenameTimeoutGet != null)
                {
                    return LoadBalancerIsAlivePageRenameTimeoutGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).LoadBalancerIsAlivePageRenameTimeout;
                }

                if (LoadBalancerIsAlivePageRenameTimeoutSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LoadBalancerIsAlivePageRenameTimeout;
                }

                return default(string);
            }

            set
            {
                if (LoadBalancerIsAlivePageRenameTimeoutSetString != null)
                {
                    LoadBalancerIsAlivePageRenameTimeoutSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).LoadBalancerIsAlivePageRenameTimeout = value;
                    return;
                }

                if (LoadBalancerIsAlivePageRenameTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _LoadBalancerIsAlivePageRenameTimeout = value;
                }

            }
        }

        private string _LoadBalancerIsAlivePageUrl;
        public Func<string> LoadBalancerIsAlivePageUrlGet;
        public Action<string> LoadBalancerIsAlivePageUrlSetString;

        string ISetupSettings.LoadBalancerIsAlivePageUrl
        {
            get
            {
                if (LoadBalancerIsAlivePageUrlGet != null)
                {
                    return LoadBalancerIsAlivePageUrlGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).LoadBalancerIsAlivePageUrl;
                }

                if (LoadBalancerIsAlivePageUrlSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LoadBalancerIsAlivePageUrl;
                }

                return default(string);
            }

            set
            {
                if (LoadBalancerIsAlivePageUrlSetString != null)
                {
                    LoadBalancerIsAlivePageUrlSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).LoadBalancerIsAlivePageUrl = value;
                    return;
                }

                if (LoadBalancerIsAlivePageUrlGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _LoadBalancerIsAlivePageUrl = value;
                }

            }
        }

        private string _MonitoringConsoleVersion;
        public Func<string> MonitoringConsoleVersionGet;
        public Action<string> MonitoringConsoleVersionSetString;

        string ISetupSettings.MonitoringConsoleVersion
        {
            get
            {
                if (MonitoringConsoleVersionGet != null)
                {
                    return MonitoringConsoleVersionGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).MonitoringConsoleVersion;
                }

                if (MonitoringConsoleVersionSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MonitoringConsoleVersion;
                }

                return default(string);
            }

            set
            {
                if (MonitoringConsoleVersionSetString != null)
                {
                    MonitoringConsoleVersionSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).MonitoringConsoleVersion = value;
                    return;
                }

                if (MonitoringConsoleVersionGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _MonitoringConsoleVersion = value;
                }

            }
        }

        private string _RedisHostName;
        public Func<string> RedisHostNameGet;
        public Action<string> RedisHostNameSetString;

        string ISetupSettings.RedisHostName
        {
            get
            {
                if (RedisHostNameGet != null)
                {
                    return RedisHostNameGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).RedisHostName;
                }

                if (RedisHostNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RedisHostName;
                }

                return default(string);
            }

            set
            {
                if (RedisHostNameSetString != null)
                {
                    RedisHostNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).RedisHostName = value;
                    return;
                }

                if (RedisHostNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RedisHostName = value;
                }

            }
        }

        private string _ReleaseDate;
        public Func<string> ReleaseDateGet;
        public Action<string> ReleaseDateSetString;

        string ISetupSettings.ReleaseDate
        {
            get
            {
                if (ReleaseDateGet != null)
                {
                    return ReleaseDateGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).ReleaseDate;
                }

                if (ReleaseDateSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReleaseDate;
                }

                return default(string);
            }

            set
            {
                if (ReleaseDateSetString != null)
                {
                    ReleaseDateSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).ReleaseDate = value;
                    return;
                }

                if (ReleaseDateGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ReleaseDate = value;
                }

            }
        }

        private string _ReleaseNumber;
        public Func<string> ReleaseNumberGet;
        public Action<string> ReleaseNumberSetString;

        string ISetupSettings.ReleaseNumber
        {
            get
            {
                if (ReleaseNumberGet != null)
                {
                    return ReleaseNumberGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).ReleaseNumber;
                }

                if (ReleaseNumberSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _ReleaseNumber;
                }

                return default(string);
            }

            set
            {
                if (ReleaseNumberSetString != null)
                {
                    ReleaseNumberSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).ReleaseNumber = value;
                    return;
                }

                if (ReleaseNumberGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _ReleaseNumber = value;
                }

            }
        }

        private string _SessionStateCookieName;
        public Func<string> SessionStateCookieNameGet;
        public Action<string> SessionStateCookieNameSetString;

        string ISetupSettings.SessionStateCookieName
        {
            get
            {
                if (SessionStateCookieNameGet != null)
                {
                    return SessionStateCookieNameGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).SessionStateCookieName;
                }

                if (SessionStateCookieNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SessionStateCookieName;
                }

                return default(string);
            }

            set
            {
                if (SessionStateCookieNameSetString != null)
                {
                    SessionStateCookieNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).SessionStateCookieName = value;
                    return;
                }

                if (SessionStateCookieNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SessionStateCookieName = value;
                }

            }
        }

        private string _SessionStateMode;
        public Func<string> SessionStateModeGet;
        public Action<string> SessionStateModeSetString;

        string ISetupSettings.SessionStateMode
        {
            get
            {
                if (SessionStateModeGet != null)
                {
                    return SessionStateModeGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).SessionStateMode;
                }

                if (SessionStateModeSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SessionStateMode;
                }

                return default(string);
            }

            set
            {
                if (SessionStateModeSetString != null)
                {
                    SessionStateModeSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).SessionStateMode = value;
                    return;
                }

                if (SessionStateModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SessionStateMode = value;
                }

            }
        }

        private string _SupervisorAppPoolName;
        public Func<string> SupervisorAppPoolNameGet;
        public Action<string> SupervisorAppPoolNameSetString;

        string ISetupSettings.SupervisorAppPoolName
        {
            get
            {
                if (SupervisorAppPoolNameGet != null)
                {
                    return SupervisorAppPoolNameGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).SupervisorAppPoolName;
                }

                if (SupervisorAppPoolNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SupervisorAppPoolName;
                }

                return default(string);
            }

            set
            {
                if (SupervisorAppPoolNameSetString != null)
                {
                    SupervisorAppPoolNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).SupervisorAppPoolName = value;
                    return;
                }

                if (SupervisorAppPoolNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SupervisorAppPoolName = value;
                }

            }
        }

        private string _SupervisorSiteName;
        public Func<string> SupervisorSiteNameGet;
        public Action<string> SupervisorSiteNameSetString;

        string ISetupSettings.SupervisorSiteName
        {
            get
            {
                if (SupervisorSiteNameGet != null)
                {
                    return SupervisorSiteNameGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).SupervisorSiteName;
                }

                if (SupervisorSiteNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SupervisorSiteName;
                }

                return default(string);
            }

            set
            {
                if (SupervisorSiteNameSetString != null)
                {
                    SupervisorSiteNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).SupervisorSiteName = value;
                    return;
                }

                if (SupervisorSiteNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SupervisorSiteName = value;
                }

            }
        }

        private string _SupervisorVirtualDirectoryName;
        public Func<string> SupervisorVirtualDirectoryNameGet;
        public Action<string> SupervisorVirtualDirectoryNameSetString;

        string ISetupSettings.SupervisorVirtualDirectoryName
        {
            get
            {
                if (SupervisorVirtualDirectoryNameGet != null)
                {
                    return SupervisorVirtualDirectoryNameGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).SupervisorVirtualDirectoryName;
                }

                if (SupervisorVirtualDirectoryNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _SupervisorVirtualDirectoryName;
                }

                return default(string);
            }

            set
            {
                if (SupervisorVirtualDirectoryNameSetString != null)
                {
                    SupervisorVirtualDirectoryNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).SupervisorVirtualDirectoryName = value;
                    return;
                }

                if (SupervisorVirtualDirectoryNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _SupervisorVirtualDirectoryName = value;
                }

            }
        }

        private string _TestCertificateName;
        public Func<string> TestCertificateNameGet;
        public Action<string> TestCertificateNameSetString;

        string ISetupSettings.TestCertificateName
        {
            get
            {
                if (TestCertificateNameGet != null)
                {
                    return TestCertificateNameGet();
                } else if (_inner != null)
                {
                    return ((ISetupSettings)_inner).TestCertificateName;
                }

                if (TestCertificateNameSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _TestCertificateName;
                }

                return default(string);
            }

            set
            {
                if (TestCertificateNameSetString != null)
                {
                    TestCertificateNameSetString(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISetupSettings)_inner).TestCertificateName = value;
                    return;
                }

                if (TestCertificateNameGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _TestCertificateName = value;
                }

            }
        }

    }
}