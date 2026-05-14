using System;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubISystemStateRepository : ISystemStateRepository 
    {
        private ISystemStateRepository _inner;

        public StubISystemStateRepository()
        {
            _inner = null;
        }

        public ISystemStateRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetStringDelegate(string systemStateName);
        public GetStringDelegate GetString;

        string ISystemStateRepository.Get(string systemStateName)
        {


            if (GetString != null)
            {
                return GetString(systemStateName);
            } else if (_inner != null)
            {
                return ((ISystemStateRepository)_inner).Get(systemStateName);
            }

            return default(string);
        }

        public delegate void SetStringStringDelegate(string systemStateName, string value);
        public SetStringStringDelegate SetStringString;

        void ISystemStateRepository.Set(string systemStateName, string value)
        {

            if (SetStringString != null)
            {
                SetStringString(systemStateName, value);
            } else if (_inner != null)
            {
                ((ISystemStateRepository)_inner).Set(systemStateName, value);
            }
        }

        public delegate DateTime? GetReviewerLastInterviewStatusChangeDelegate();
        public GetReviewerLastInterviewStatusChangeDelegate GetReviewerLastInterviewStatusChange;

        DateTime? ISystemStateRepository.GetReviewerLastInterviewStatusChange()
        {


            if (GetReviewerLastInterviewStatusChange != null)
            {
                return GetReviewerLastInterviewStatusChange();
            } else if (_inner != null)
            {
                return ((ISystemStateRepository)_inner).GetReviewerLastInterviewStatusChange();
            }

            return default(DateTime?);
        }

        public delegate void SetReviewerLastInterviewStatusChangeDateTimeDelegate(DateTime value);
        public SetReviewerLastInterviewStatusChangeDateTimeDelegate SetReviewerLastInterviewStatusChangeDateTime;

        void ISystemStateRepository.SetReviewerLastInterviewStatusChange(DateTime value)
        {

            if (SetReviewerLastInterviewStatusChangeDateTime != null)
            {
                SetReviewerLastInterviewStatusChangeDateTime(value);
            } else if (_inner != null)
            {
                ((ISystemStateRepository)_inner).SetReviewerLastInterviewStatusChange(value);
            }
        }

    }
}