using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIQuotaClusteringSettings : IQuotaClusteringSettings 
    {
        private IQuotaClusteringSettings _inner;

        public StubIQuotaClusteringSettings()
        {
            _inner = null;
        }

        public IQuotaClusteringSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private bool _Enabled;
        public Func<bool> EnabledGet;
        public Action<bool> EnabledSetBoolean;

        bool IQuotaClusteringSettings.Enabled
        {
            get
            {
                if (EnabledGet != null)
                {
                    return EnabledGet();
                } else if (_inner != null)
                {
                    return ((IQuotaClusteringSettings)_inner).Enabled;
                }

                if (EnabledSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _Enabled;
                }

                return default(bool);
            }

            set
            {
                if (EnabledSetBoolean != null)
                {
                    EnabledSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IQuotaClusteringSettings)_inner).Enabled = value;
                    return;
                }

                if (EnabledGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _Enabled = value;
                }

            }
        }

    }
}