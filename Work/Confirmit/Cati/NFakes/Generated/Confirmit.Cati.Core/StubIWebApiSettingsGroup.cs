using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubIWebApiSettingsGroup : IWebApiSettingsGroup 
    {
        private IWebApiSettingsGroup _inner;

        public StubIWebApiSettingsGroup()
        {
            _inner = null;
        }

        public IWebApiSettingsGroup Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void OnChangedDelegate();
        public OnChangedDelegate OnChanged;

        void ISystemSettingsNotifyChanged.OnChanged()
        {

            if (OnChanged != null)
            {
                OnChanged();
            } else if (_inner != null)
            {
                ((ISystemSettingsNotifyChanged)_inner).OnChanged();
            }
        }

        private bool _EnableSwagger;
        public Func<bool> EnableSwaggerGet;
        public Action<bool> EnableSwaggerSetBoolean;

        bool IWebApiSettings.EnableSwagger
        {
            get
            {
                if (EnableSwaggerGet != null)
                {
                    return EnableSwaggerGet();
                } else if (_inner != null)
                {
                    return ((IWebApiSettings)_inner).EnableSwagger;
                }

                if (EnableSwaggerSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _EnableSwagger;
                }

                return default(bool);
            }

            set
            {
                if (EnableSwaggerSetBoolean != null)
                {
                    EnableSwaggerSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWebApiSettings)_inner).EnableSwagger = value;
                    return;
                }

                if (EnableSwaggerGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _EnableSwagger = value;
                }

            }
        }

        private int _PageSize;
        public Func<int> PageSizeGet;
        public Action<int> PageSizeSetInt32;

        int IWebApiSettings.PageSize
        {
            get
            {
                if (PageSizeGet != null)
                {
                    return PageSizeGet();
                } else if (_inner != null)
                {
                    return ((IWebApiSettings)_inner).PageSize;
                }

                if (PageSizeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _PageSize;
                }

                return default(int);
            }

            set
            {
                if (PageSizeSetInt32 != null)
                {
                    PageSizeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWebApiSettings)_inner).PageSize = value;
                    return;
                }

                if (PageSizeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _PageSize = value;
                }

            }
        }

        private bool _RateLimiting;
        public Func<bool> RateLimitingGet;
        public Action<bool> RateLimitingSetBoolean;

        bool IWebApiSettings.RateLimiting
        {
            get
            {
                if (RateLimitingGet != null)
                {
                    return RateLimitingGet();
                } else if (_inner != null)
                {
                    return ((IWebApiSettings)_inner).RateLimiting;
                }

                if (RateLimitingSetBoolean == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _RateLimiting;
                }

                return default(bool);
            }

            set
            {
                if (RateLimitingSetBoolean != null)
                {
                    RateLimitingSetBoolean(value);
                    return;
                } else if (_inner != null)
                {
                    ((IWebApiSettings)_inner).RateLimiting = value;
                    return;
                }

                if (RateLimitingGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _RateLimiting = value;
                }

            }
        }

    }
}