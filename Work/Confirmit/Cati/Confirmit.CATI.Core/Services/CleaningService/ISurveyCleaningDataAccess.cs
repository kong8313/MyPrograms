using System;
using System.Collections.Generic;

namespace Confirmit.CATI.Core.Services.CleaningService
{
    public interface ISurveyCleaningDataAccess
    {
        List<CleaningServiceEmailInfo> GetSurveysWhichAreReadyForNotice(DateTime lastTouchTime);
        List<CleaningServiceEmailInfo> GetSurveysWhichAreReadyForCleanup(DateTime lastTouchTime, DateTime lastSentNoticeTime);
        void CleanSurvey(int surveyId);
    }
}