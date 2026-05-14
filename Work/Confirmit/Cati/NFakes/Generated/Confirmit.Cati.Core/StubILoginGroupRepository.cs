using System;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories.Interfaces.Fakes
{
    public class StubILoginGroupRepository : ILoginGroupRepository 
    {
        private ILoginGroupRepository _inner;

        public StubILoginGroupRepository()
        {
            _inner = null;
        }

        public ILoginGroupRepository Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool IsResourceLoggedIntoSurveyInt32Int32Delegate(int resourceId, int surveySid);
        public IsResourceLoggedIntoSurveyInt32Int32Delegate IsResourceLoggedIntoSurveyInt32Int32;

        bool ILoginGroupRepository.IsResourceLoggedIntoSurvey(int resourceId, int surveySid)
        {


            if (IsResourceLoggedIntoSurveyInt32Int32 != null)
            {
                return IsResourceLoggedIntoSurveyInt32Int32(resourceId, surveySid);
            } else if (_inner != null)
            {
                return ((ILoginGroupRepository)_inner).IsResourceLoggedIntoSurvey(resourceId, surveySid);
            }

            return default(bool);
        }

        public delegate bool IsResourceReadyForCallInSurveyInt32Int32Delegate(int resourceId, int surveySid);
        public IsResourceReadyForCallInSurveyInt32Int32Delegate IsResourceReadyForCallInSurveyInt32Int32;

        bool ILoginGroupRepository.IsResourceReadyForCallInSurvey(int resourceId, int surveySid)
        {


            if (IsResourceReadyForCallInSurveyInt32Int32 != null)
            {
                return IsResourceReadyForCallInSurveyInt32Int32(resourceId, surveySid);
            } else if (_inner != null)
            {
                return ((ILoginGroupRepository)_inner).IsResourceReadyForCallInSurvey(resourceId, surveySid);
            }

            return default(bool);
        }

        public delegate bool IsAnyoneLoggedIntoSurveyInt32Delegate(int surveySid);
        public IsAnyoneLoggedIntoSurveyInt32Delegate IsAnyoneLoggedIntoSurveyInt32;

        bool ILoginGroupRepository.IsAnyoneLoggedIntoSurvey(int surveySid)
        {


            if (IsAnyoneLoggedIntoSurveyInt32 != null)
            {
                return IsAnyoneLoggedIntoSurveyInt32(surveySid);
            } else if (_inner != null)
            {
                return ((ILoginGroupRepository)_inner).IsAnyoneLoggedIntoSurvey(surveySid);
            }

            return default(bool);
        }

        public delegate bool IsAnyoneLoggedIntoSurveyInt32Int32Delegate(int surveySid, int agentTypeIndex);
        public IsAnyoneLoggedIntoSurveyInt32Int32Delegate IsAnyoneLoggedIntoSurveyInt32Int32;

        bool ILoginGroupRepository.IsAnyoneLoggedIntoSurvey(int surveySid, int agentTypeIndex)
        {


            if (IsAnyoneLoggedIntoSurveyInt32Int32 != null)
            {
                return IsAnyoneLoggedIntoSurveyInt32Int32(surveySid, agentTypeIndex);
            } else if (_inner != null)
            {
                return ((ILoginGroupRepository)_inner).IsAnyoneLoggedIntoSurvey(surveySid, agentTypeIndex);
            }

            return default(bool);
        }

        public delegate bool IsAnyoneReadyForCallInSurveyInt32Int32Delegate(int surveySid, int agentTypeIndex);
        public IsAnyoneReadyForCallInSurveyInt32Int32Delegate IsAnyoneReadyForCallInSurveyInt32Int32;

        bool ILoginGroupRepository.IsAnyoneReadyForCallInSurvey(int surveySid, int agentTypeIndex)
        {


            if (IsAnyoneReadyForCallInSurveyInt32Int32 != null)
            {
                return IsAnyoneReadyForCallInSurveyInt32Int32(surveySid, agentTypeIndex);
            } else if (_inner != null)
            {
                return ((ILoginGroupRepository)_inner).IsAnyoneReadyForCallInSurvey(surveySid, agentTypeIndex);
            }

            return default(bool);
        }

    }
}