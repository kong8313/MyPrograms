using System;

namespace Confirmit.CATI.Core.Repositories
{
    public class InterviewerGroupNotFoundException : Exception
    {
        private readonly string _interviewerGroupName;
        private readonly int _interviewerGroupId;

        public InterviewerGroupNotFoundException(int interviewerGroupId)
            : base(string.Format("Interviewer group {0} is not found.", interviewerGroupId))
        {
            _interviewerGroupId = interviewerGroupId;
        }

        public InterviewerGroupNotFoundException(string interviewerGroupName)
            : base(string.Format("Interviewer group {0} is not found.", interviewerGroupName))
        {
            _interviewerGroupName = interviewerGroupName;
        }

        public int InterviewerGroupId
        {
            get { return _interviewerGroupId; }
        }

        public string InterviewerGroupName
        {
            get { return _interviewerGroupName; }
        }
    }
}