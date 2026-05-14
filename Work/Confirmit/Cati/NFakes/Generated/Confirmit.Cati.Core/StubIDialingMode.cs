using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using Confirmit.CATI.Core.Telephony.DialingWorkflow;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.DialingWorkflow.Fakes
{
    public class StubIDialingMode : IDialingMode 
    {
        private IDialingMode _inner;

        public StubIDialingMode()
        {
            _inner = null;
        }

        public IDialingMode Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DialerErrorCode LoginBvPersonEntityBvTasksEntityBvSurveyEntityStringIEnumerableOfKeyValuePairOfStringStringDelegate(BvPersonEntity person, BvTasksEntity task, BvSurveyEntity survey, string extensionNumber, IEnumerable<KeyValuePair<string, string>> personDialerAttributes);
        public LoginBvPersonEntityBvTasksEntityBvSurveyEntityStringIEnumerableOfKeyValuePairOfStringStringDelegate LoginBvPersonEntityBvTasksEntityBvSurveyEntityStringIEnumerableOfKeyValuePairOfStringString;

        DialerErrorCode IDialingMode.Login(BvPersonEntity person, BvTasksEntity task, BvSurveyEntity survey, string extensionNumber, IEnumerable<KeyValuePair<string, string>> personDialerAttributes)
        {


            if (LoginBvPersonEntityBvTasksEntityBvSurveyEntityStringIEnumerableOfKeyValuePairOfStringString != null)
            {
                return LoginBvPersonEntityBvTasksEntityBvSurveyEntityStringIEnumerableOfKeyValuePairOfStringString(person, task, survey, extensionNumber, personDialerAttributes);
            } else if (_inner != null)
            {
                return ((IDialingMode)_inner).Login(person, task, survey, extensionNumber, personDialerAttributes);
            }

            return default(DialerErrorCode);
        }

        public delegate void BeforeStartInterviewBvTasksEntityBvPersonEntityDelegate(BvTasksEntity task, BvPersonEntity person);
        public BeforeStartInterviewBvTasksEntityBvPersonEntityDelegate BeforeStartInterviewBvTasksEntityBvPersonEntity;

        void IDialingMode.BeforeStartInterview(BvTasksEntity task, BvPersonEntity person)
        {

            if (BeforeStartInterviewBvTasksEntityBvPersonEntity != null)
            {
                BeforeStartInterviewBvTasksEntityBvPersonEntity(task, person);
            } else if (_inner != null)
            {
                ((IDialingMode)_inner).BeforeStartInterview(task, person);
            }
        }

        public delegate void StartInterviewInt32Int32BvSurveyEntityBvInterviewEntityInt32Delegate(int personId, int dialerId, BvSurveyEntity survey, BvInterviewEntity interview, int timezoneId);
        public StartInterviewInt32Int32BvSurveyEntityBvInterviewEntityInt32Delegate StartInterviewInt32Int32BvSurveyEntityBvInterviewEntityInt32;

        void IDialingMode.StartInterview(int personId, int dialerId, BvSurveyEntity survey, BvInterviewEntity interview, int timezoneId)
        {

            if (StartInterviewInt32Int32BvSurveyEntityBvInterviewEntityInt32 != null)
            {
                StartInterviewInt32Int32BvSurveyEntityBvInterviewEntityInt32(personId, dialerId, survey, interview, timezoneId);
            } else if (_inner != null)
            {
                ((IDialingMode)_inner).StartInterview(personId, dialerId, survey, interview, timezoneId);
            }
        }

        public delegate void CheckPersonCanLoginToDialerBvPersonEntityDelegate(BvPersonEntity person);
        public CheckPersonCanLoginToDialerBvPersonEntityDelegate CheckPersonCanLoginToDialerBvPersonEntity;

        void IDialingMode.CheckPersonCanLoginToDialer(BvPersonEntity person)
        {

            if (CheckPersonCanLoginToDialerBvPersonEntity != null)
            {
                CheckPersonCanLoginToDialerBvPersonEntity(person);
            } else if (_inner != null)
            {
                ((IDialingMode)_inner).CheckPersonCanLoginToDialer(person);
            }
        }

    }
}