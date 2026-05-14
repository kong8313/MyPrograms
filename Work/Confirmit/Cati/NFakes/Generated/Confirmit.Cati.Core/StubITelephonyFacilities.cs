using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubITelephonyFacilities : ITelephonyFacilities 
    {
        private ITelephonyFacilities _inner;

        public StubITelephonyFacilities()
        {
            _inner = null;
        }

        public ITelephonyFacilities Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<LogFileInfo> GetLogFilesInt32Delegate(int dialerId);
        public GetLogFilesInt32Delegate GetLogFilesInt32;

        IEnumerable<LogFileInfo> ITelephonyFacilities.GetLogFiles(int dialerId)
        {


            if (GetLogFilesInt32 != null)
            {
                return GetLogFilesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyFacilities)_inner).GetLogFiles(dialerId);
            }

            return default(IEnumerable<LogFileInfo>);
        }

        public delegate byte[] GetLogFileBodyZippedInt32StringDelegate(int dialerId, string fileName);
        public GetLogFileBodyZippedInt32StringDelegate GetLogFileBodyZippedInt32String;

        byte[] ITelephonyFacilities.GetLogFileBodyZipped(int dialerId, string fileName)
        {


            if (GetLogFileBodyZippedInt32String != null)
            {
                return GetLogFileBodyZippedInt32String(dialerId, fileName);
            } else if (_inner != null)
            {
                return ((ITelephonyFacilities)_inner).GetLogFileBodyZipped(dialerId, fileName);
            }

            return default(byte[]);
        }

        public delegate string GetDialerVersionInt32Delegate(int dialerId);
        public GetDialerVersionInt32Delegate GetDialerVersionInt32;

        string ITelephonyFacilities.GetDialerVersion(int dialerId)
        {


            if (GetDialerVersionInt32 != null)
            {
                return GetDialerVersionInt32(dialerId);
            } else if (_inner != null)
            {
                return ((ITelephonyFacilities)_inner).GetDialerVersion(dialerId);
            }

            return default(string);
        }

    }
}