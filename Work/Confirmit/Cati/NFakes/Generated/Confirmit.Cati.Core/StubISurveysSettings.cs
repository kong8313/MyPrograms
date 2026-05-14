using System;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.SystemSettings.Fakes
{
    public class StubISurveysSettings : ISurveysSettings 
    {
        private ISurveysSettings _inner;

        public StubISurveysSettings()
        {
            _inner = null;
        }

        public ISurveysSettings Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        private int _DefaultCallDeliveryMode;
        public Func<int> DefaultCallDeliveryModeGet;
        public Action<int> DefaultCallDeliveryModeSetInt32;

        int ISurveysSettings.DefaultCallDeliveryMode
        {
            get
            {
                if (DefaultCallDeliveryModeGet != null)
                {
                    return DefaultCallDeliveryModeGet();
                } else if (_inner != null)
                {
                    return ((ISurveysSettings)_inner).DefaultCallDeliveryMode;
                }

                if (DefaultCallDeliveryModeSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _DefaultCallDeliveryMode;
                }

                return default(int);
            }

            set
            {
                if (DefaultCallDeliveryModeSetInt32 != null)
                {
                    DefaultCallDeliveryModeSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((ISurveysSettings)_inner).DefaultCallDeliveryMode = value;
                    return;
                }

                if (DefaultCallDeliveryModeGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _DefaultCallDeliveryMode = value;
                }

            }
        }

    }
}