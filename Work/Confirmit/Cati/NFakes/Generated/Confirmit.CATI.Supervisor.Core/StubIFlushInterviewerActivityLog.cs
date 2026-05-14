using System;
using Confirmit.CATI.Supervisor.Core.Management;

namespace Confirmit.CATI.Supervisor.Core.Management.Fakes
{
    public class StubIFlushInterviewerActivityLog : IFlushInterviewerActivityLog 
    {
        private IFlushInterviewerActivityLog _inner;

        public StubIFlushInterviewerActivityLog()
        {
            _inner = null;
        }

        public IFlushInterviewerActivityLog Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void FlushDelegate();
        public FlushDelegate Flush;

        void IFlushInterviewerActivityLog.Flush()
        {

            if (Flush != null)
            {
                Flush();
            } else if (_inner != null)
            {
                ((IFlushInterviewerActivityLog)_inner).Flush();
            }
        }

    }
}