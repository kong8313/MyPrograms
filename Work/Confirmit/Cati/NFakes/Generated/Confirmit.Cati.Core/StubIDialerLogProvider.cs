using System;
using Confirmit.CATI.Core.Telephony;
using System.Collections.Generic;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerLogProvider : IDialerLogProvider 
    {
        private IDialerLogProvider _inner;

        public StubIDialerLogProvider()
        {
            _inner = null;
        }

        public IDialerLogProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<LogFileInfo> GetLogFilesInt32Delegate(int dialerId);
        public GetLogFilesInt32Delegate GetLogFilesInt32;

        IEnumerable<LogFileInfo> IDialerLogProvider.GetLogFiles(int dialerId)
        {


            if (GetLogFilesInt32 != null)
            {
                return GetLogFilesInt32(dialerId);
            } else if (_inner != null)
            {
                return ((IDialerLogProvider)_inner).GetLogFiles(dialerId);
            }

            return default(IEnumerable<LogFileInfo>);
        }

        public delegate byte[] GetLogFileBodyZippedInt32StringDelegate(int dialerId, string fileName);
        public GetLogFileBodyZippedInt32StringDelegate GetLogFileBodyZippedInt32String;

        byte[] IDialerLogProvider.GetLogFileBodyZipped(int dialerId, string fileName)
        {


            if (GetLogFileBodyZippedInt32String != null)
            {
                return GetLogFileBodyZippedInt32String(dialerId, fileName);
            } else if (_inner != null)
            {
                return ((IDialerLogProvider)_inner).GetLogFileBodyZipped(dialerId, fileName);
            }

            return default(byte[]);
        }

    }
}