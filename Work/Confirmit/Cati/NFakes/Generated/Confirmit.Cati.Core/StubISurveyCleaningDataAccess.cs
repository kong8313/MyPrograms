using System;
using Confirmit.CATI.Core.Services.CleaningService;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.CleaningService.Fakes
{
    public class StubISurveyCleaningDataAccess : ISurveyCleaningDataAccess 
    {
        private ISurveyCleaningDataAccess _inner;

        public StubISurveyCleaningDataAccess()
        {
            _inner = null;
        }

        public ISurveyCleaningDataAccess Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate List<CleaningServiceEmailInfo> GetSurveysWhichAreReadyForNoticeDateTimeDelegate(DateTime lastTouchTime);
        public GetSurveysWhichAreReadyForNoticeDateTimeDelegate GetSurveysWhichAreReadyForNoticeDateTime;

        List<CleaningServiceEmailInfo> ISurveyCleaningDataAccess.GetSurveysWhichAreReadyForNotice(DateTime lastTouchTime)
        {


            if (GetSurveysWhichAreReadyForNoticeDateTime != null)
            {
                return GetSurveysWhichAreReadyForNoticeDateTime(lastTouchTime);
            } else if (_inner != null)
            {
                return ((ISurveyCleaningDataAccess)_inner).GetSurveysWhichAreReadyForNotice(lastTouchTime);
            }

            return default(List<CleaningServiceEmailInfo>);
        }

        public delegate List<CleaningServiceEmailInfo> GetSurveysWhichAreReadyForCleanupDateTimeDateTimeDelegate(DateTime lastTouchTime, DateTime lastSentNoticeTime);
        public GetSurveysWhichAreReadyForCleanupDateTimeDateTimeDelegate GetSurveysWhichAreReadyForCleanupDateTimeDateTime;

        List<CleaningServiceEmailInfo> ISurveyCleaningDataAccess.GetSurveysWhichAreReadyForCleanup(DateTime lastTouchTime, DateTime lastSentNoticeTime)
        {


            if (GetSurveysWhichAreReadyForCleanupDateTimeDateTime != null)
            {
                return GetSurveysWhichAreReadyForCleanupDateTimeDateTime(lastTouchTime, lastSentNoticeTime);
            } else if (_inner != null)
            {
                return ((ISurveyCleaningDataAccess)_inner).GetSurveysWhichAreReadyForCleanup(lastTouchTime, lastSentNoticeTime);
            }

            return default(List<CleaningServiceEmailInfo>);
        }

        public delegate void CleanSurveyInt32Delegate(int surveyId);
        public CleanSurveyInt32Delegate CleanSurveyInt32;

        void ISurveyCleaningDataAccess.CleanSurvey(int surveyId)
        {

            if (CleanSurveyInt32 != null)
            {
                CleanSurveyInt32(surveyId);
            } else if (_inner != null)
            {
                ((ISurveyCleaningDataAccess)_inner).CleanSurvey(surveyId);
            }
        }

    }
}