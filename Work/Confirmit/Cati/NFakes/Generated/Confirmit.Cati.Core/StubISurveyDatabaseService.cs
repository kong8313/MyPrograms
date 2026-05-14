using System;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubISurveyDatabaseService : ISurveyDatabaseService 
    {
        private ISurveyDatabaseService _inner;

        public StubISurveyDatabaseService()
        {
            _inner = null;
        }

        public ISurveyDatabaseService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate int IncrementCallAttemptCountInt32Int32Delegate(int surveyId, int interviewId);
        public IncrementCallAttemptCountInt32Int32Delegate IncrementCallAttemptCountInt32Int32;

        int ISurveyDatabaseService.IncrementCallAttemptCount(int surveyId, int interviewId)
        {


            if (IncrementCallAttemptCountInt32Int32 != null)
            {
                return IncrementCallAttemptCountInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseService)_inner).IncrementCallAttemptCount(surveyId, interviewId);
            }

            return default(int);
        }

        public delegate int GetCallAttemptCountInt32Int32Delegate(int surveyId, int interviewId);
        public GetCallAttemptCountInt32Int32Delegate GetCallAttemptCountInt32Int32;

        int ISurveyDatabaseService.GetCallAttemptCount(int surveyId, int interviewId)
        {


            if (GetCallAttemptCountInt32Int32 != null)
            {
                return GetCallAttemptCountInt32Int32(surveyId, interviewId);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseService)_inner).GetCallAttemptCount(surveyId, interviewId);
            }

            return default(int);
        }

        public delegate void UpdateItsInt32Int32Int32Delegate(int surveyId, int interviewId, int its);
        public UpdateItsInt32Int32Int32Delegate UpdateItsInt32Int32Int32;

        void ISurveyDatabaseService.UpdateIts(int surveyId, int interviewId, int its)
        {

            if (UpdateItsInt32Int32Int32 != null)
            {
                UpdateItsInt32Int32Int32(surveyId, interviewId, its);
            } else if (_inner != null)
            {
                ((ISurveyDatabaseService)_inner).UpdateIts(surveyId, interviewId, its);
            }
        }

        public delegate void UpdateTimeZoneIdInt32Int32Int32Delegate(int surveyId, int interviewId, int timeZoneId);
        public UpdateTimeZoneIdInt32Int32Int32Delegate UpdateTimeZoneIdInt32Int32Int32;

        void ISurveyDatabaseService.UpdateTimeZoneId(int surveyId, int interviewId, int timeZoneId)
        {

            if (UpdateTimeZoneIdInt32Int32Int32 != null)
            {
                UpdateTimeZoneIdInt32Int32Int32(surveyId, interviewId, timeZoneId);
            } else if (_inner != null)
            {
                ((ISurveyDatabaseService)_inner).UpdateTimeZoneId(surveyId, interviewId, timeZoneId);
            }
        }

        public delegate List<string> ProcessRespondentFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntityDelegate(int surveyId, int interviewId, List<BvHistoryCustomFieldsEntity> fields);
        public ProcessRespondentFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntityDelegate ProcessRespondentFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntity;

        List<string> ISurveyDatabaseService.ProcessRespondentFieldsBatch(int surveyId, int interviewId, List<BvHistoryCustomFieldsEntity> fields)
        {


            if (ProcessRespondentFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntity != null)
            {
                return ProcessRespondentFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntity(surveyId, interviewId, fields);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseService)_inner).ProcessRespondentFieldsBatch(surveyId, interviewId, fields);
            }

            return default(List<string>);
        }

        public delegate List<string> ProcessCallHistoryLoopFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntityDelegate(int surveyId, int interviewId, List<BvHistoryCustomFieldsEntity> fields);
        public ProcessCallHistoryLoopFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntityDelegate ProcessCallHistoryLoopFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntity;

        List<string> ISurveyDatabaseService.ProcessCallHistoryLoopFieldsBatch(int surveyId, int interviewId, List<BvHistoryCustomFieldsEntity> fields)
        {


            if (ProcessCallHistoryLoopFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntity != null)
            {
                return ProcessCallHistoryLoopFieldsBatchInt32Int32ListOfBvHistoryCustomFieldsEntity(surveyId, interviewId, fields);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseService)_inner).ProcessCallHistoryLoopFieldsBatch(surveyId, interviewId, fields);
            }

            return default(List<string>);
        }

        public delegate string ProcessResponseFieldInt32Int32BvHistoryCustomFieldsEntityDelegate(int surveyId, int interviewId, BvHistoryCustomFieldsEntity field);
        public ProcessResponseFieldInt32Int32BvHistoryCustomFieldsEntityDelegate ProcessResponseFieldInt32Int32BvHistoryCustomFieldsEntity;

        string ISurveyDatabaseService.ProcessResponseField(int surveyId, int interviewId, BvHistoryCustomFieldsEntity field)
        {


            if (ProcessResponseFieldInt32Int32BvHistoryCustomFieldsEntity != null)
            {
                return ProcessResponseFieldInt32Int32BvHistoryCustomFieldsEntity(surveyId, interviewId, field);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseService)_inner).ProcessResponseField(surveyId, interviewId, field);
            }

            return default(string);
        }

        public delegate List<string> GetCustomFieldValuesInt32Int32Delegate(int surveySID, int interviewID);
        public GetCustomFieldValuesInt32Int32Delegate GetCustomFieldValuesInt32Int32;

        List<string> ISurveyDatabaseService.GetCustomFieldValues(int surveySID, int interviewID)
        {


            if (GetCustomFieldValuesInt32Int32 != null)
            {
                return GetCustomFieldValuesInt32Int32(surveySID, interviewID);
            } else if (_inner != null)
            {
                return ((ISurveyDatabaseService)_inner).GetCustomFieldValues(surveySID, interviewID);
            }

            return default(List<string>);
        }

    }
}