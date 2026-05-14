using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIDebugSettings : IDebugSettings 
    {
        private IDebugSettings _inner;

        public StubIDebugSettings()
        {
            _inner = null;
        }

        public IDebugSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _BackendStartup;
        public Func<bool> BackendStartupGet;
        public Action<bool> BackendStartupSetBoolean;

        bool IDebugSettings.BackendStartup
        {
            get
            {
                if (BackendStartupGet != null)
                {
                    return BackendStartupGet();
                } else if (_inner != null)
                {
                    return ((IDebugSettings)_inner).BackendStartup;
                }

                if (BackendStartupSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _BackendStartup;
                }

                return default(bool);
            }

            set
            {
                if (BackendStartupSetBoolean != null)
                {
                    BackendStartupSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDebugSettings)_inner).BackendStartup = value;
                    return;
                }

                if (BackendStartupGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _BackendStartup = value;
                }

            }
        }

        private bool _PublishMetadataForExternalWCFServices;
        public Func<bool> PublishMetadataForExternalWCFServicesGet;
        public Action<bool> PublishMetadataForExternalWCFServicesSetBoolean;

        bool IDebugSettings.PublishMetadataForExternalWCFServices
        {
            get
            {
                if (PublishMetadataForExternalWCFServicesGet != null)
                {
                    return PublishMetadataForExternalWCFServicesGet();
                } else if (_inner != null)
                {
                    return ((IDebugSettings)_inner).PublishMetadataForExternalWCFServices;
                }

                if (PublishMetadataForExternalWCFServicesSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _PublishMetadataForExternalWCFServices;
                }

                return default(bool);
            }

            set
            {
                if (PublishMetadataForExternalWCFServicesSetBoolean != null)
                {
                    PublishMetadataForExternalWCFServicesSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDebugSettings)_inner).PublishMetadataForExternalWCFServices = value;
                    return;
                }

                if (PublishMetadataForExternalWCFServicesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _PublishMetadataForExternalWCFServices = value;
                }

            }
        }

        private bool _PublishMetadataForInternalWCFServices;
        public Func<bool> PublishMetadataForInternalWCFServicesGet;
        public Action<bool> PublishMetadataForInternalWCFServicesSetBoolean;

        bool IDebugSettings.PublishMetadataForInternalWCFServices
        {
            get
            {
                if (PublishMetadataForInternalWCFServicesGet != null)
                {
                    return PublishMetadataForInternalWCFServicesGet();
                } else if (_inner != null)
                {
                    return ((IDebugSettings)_inner).PublishMetadataForInternalWCFServices;
                }

                if (PublishMetadataForInternalWCFServicesSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _PublishMetadataForInternalWCFServices;
                }

                return default(bool);
            }

            set
            {
                if (PublishMetadataForInternalWCFServicesSetBoolean != null)
                {
                    PublishMetadataForInternalWCFServicesSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IDebugSettings)_inner).PublishMetadataForInternalWCFServices = value;
                    return;
                }

                if (PublishMetadataForInternalWCFServicesGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _PublishMetadataForInternalWCFServices = value;
                }

            }
        }

    }
}