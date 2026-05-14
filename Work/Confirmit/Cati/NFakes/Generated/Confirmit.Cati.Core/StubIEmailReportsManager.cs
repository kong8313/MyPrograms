using System;
using Confirmit.CATI.Core.EmailReports;

namespace Confirmit.CATI.Core.EmailReports.Fakes
{
    public class StubIEmailReportsManager : IEmailReportsManager 
    {
        private IEmailReportsManager _inner;

        public StubIEmailReportsManager()
        {
            _inner = null;
        }

        public IEmailReportsManager Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ProcessReportsDelegate();
        public ProcessReportsDelegate ProcessReports;

        void IEmailReportsManager.ProcessReports()
        {

            if (ProcessReports != null)
            {
                ProcessReports();
            } else if (_inner != null)
            {
                ((IEmailReportsManager)_inner).ProcessReports();
            }
        }

    }
}