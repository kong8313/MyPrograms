using System;
using DialerCommon;

namespace DialerCommon.Fakes
{
    public class StubIDialerCommonSettings : IDialerCommonSettings 
    {
        private IDialerCommonSettings _inner;

        public StubIDialerCommonSettings()
        {
            _inner = null;
        }

        public IDialerCommonSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private string _MonitorRoot;
        public Func<string> MonitorRootGet;
        public Action<string> MonitorRootSetString;

        string IDialerCommonSettings.MonitorRoot
        {
            get
            {
                if (MonitorRootGet != null)
                {
                    return MonitorRootGet();
                } else if (_inner != null)
                {
                    return ((IDialerCommonSettings)_inner).MonitorRoot;
                }

                if (MonitorRootSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _MonitorRoot;
                }

                return default(string);
            }

        }

        private string _UrlRoot;
        public Func<string> UrlRootGet;
        public Action<string> UrlRootSetString;

        string IDialerCommonSettings.UrlRoot
        {
            get
            {
                if (UrlRootGet != null)
                {
                    return UrlRootGet();
                } else if (_inner != null)
                {
                    return ((IDialerCommonSettings)_inner).UrlRoot;
                }

                if (UrlRootSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _UrlRoot;
                }

                return default(string);
            }

        }

        private string _FileNamePattern;
        public Func<string> FileNamePatternGet;
        public Action<string> FileNamePatternSetString;

        string IDialerCommonSettings.FileNamePattern
        {
            get
            {
                if (FileNamePatternGet != null)
                {
                    return FileNamePatternGet();
                } else if (_inner != null)
                {
                    return ((IDialerCommonSettings)_inner).FileNamePattern;
                }

                if (FileNamePatternSetString == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _FileNamePattern;
                }

                return default(string);
            }

        }

        private int _LogHealthCheckPeriodInMinutes;
        public Func<int> LogHealthCheckPeriodInMinutesGet;
        public Action<int> LogHealthCheckPeriodInMinutesSetInt32;

        int IDialerCommonSettings.LogHealthCheckPeriodInMinutes
        {
            get
            {
                if (LogHealthCheckPeriodInMinutesGet != null)
                {
                    return LogHealthCheckPeriodInMinutesGet();
                } else if (_inner != null)
                {
                    return ((IDialerCommonSettings)_inner).LogHealthCheckPeriodInMinutes;
                }

                if (LogHealthCheckPeriodInMinutesSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _LogHealthCheckPeriodInMinutes;
                }

                return default(int);
            }

        }

    }
}