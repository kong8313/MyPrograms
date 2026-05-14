using System;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Telephony;

namespace BvCallHandlerLibrary.Tools.Fakes
{
    public class StubIMnTciTools : IMnTciTools 
    {
        private IMnTciTools _inner;

        public StubIMnTciTools()
        {
            _inner = null;
        }

        public IMnTciTools Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IDialerRecordingAPI CreateDialerRecordingInt32Delegate(int dialerId);
        public CreateDialerRecordingInt32Delegate CreateDialerRecordingInt32;

        IDialerRecordingAPI IMnTciTools.CreateDialerRecording(int dialerId)
        {


            if (CreateDialerRecordingInt32 != null)
            {
                return CreateDialerRecordingInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IMnTciTools)_inner).CreateDialerRecording(dialerId);
            }

            return default(IDialerRecordingAPI);
        }

        public delegate bool IsDialerConfiguredDelegate();
        public IsDialerConfiguredDelegate IsDialerConfigured;

        bool IMnTciTools.IsDialerConfigured()
        {


            if (IsDialerConfigured != null)
            {
                return IsDialerConfigured();
            } else if (_inner != null)
            {
                return ((IMnTciTools)_inner).IsDialerConfigured();
            }

            return default(bool);
        }

        public delegate bool DoesCompanyUseTelephonyDelegate();
        public DoesCompanyUseTelephonyDelegate DoesCompanyUseTelephony;

        bool IMnTciTools.DoesCompanyUseTelephony()
        {


            if (DoesCompanyUseTelephony != null)
            {
                return DoesCompanyUseTelephony();
            } else if (_inner != null)
            {
                return ((IMnTciTools)_inner).DoesCompanyUseTelephony();
            }

            return default(bool);
        }

    }
}