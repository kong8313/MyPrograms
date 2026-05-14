using System;

namespace Confirmit.CATI.Core.Repositories
{
    public class InterviewerNotFoundException : Exception
    {
        private readonly string _interviewerName;
        private readonly int _interviewerId;

        public InterviewerNotFoundException(int interviewerId) : base(string.Format("Interviewer {0} is not found.", interviewerId))
        {
            _interviewerId = interviewerId;
        }

        public InterviewerNotFoundException(string interviewerName): base(string.Format("Interviewer {0} is not found.", interviewerName))
        {
            _interviewerName = interviewerName;
        }

        public int InterviewerId
        {
            get { return _interviewerId; }
        }

        public string InterviewerName
        {
            get { return _interviewerName; }
        }
    }
}