using System;
using System.Linq;
using Confirmit.CATI.Core.Misc;
using System.Collections.Generic;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;

namespace Confirmit.CATI.Core.Services.CleaningService
{
    public class SurveyCleaningDataAccess : ISurveyCleaningDataAccess
    {
        private const int BatchSize = 10000;
        private readonly ISurveyCleaningConfirmitDataAccess _surveyCleaningConfirmitDataAccess;

        public SurveyCleaningDataAccess(ISurveyCleaningConfirmitDataAccess surveyCleaningConfirmitDataAccess)
        {
            _surveyCleaningConfirmitDataAccess = surveyCleaningConfirmitDataAccess;
        }

        /// <summary>
        /// This method return list of surveys which can be planed to cleanup but we didn't send notice to users about it.
        /// </summary>
        /// <returns></returns>
        public List<CleaningServiceEmailInfo> GetSurveysWhichAreReadyForNotice(DateTime lastTouchTime)
        {
            var surveys = BvSpSurveyCleanup_GetSurveysWhichAreReadyForNoticeAdapter.ExecuteEntityList(lastTouchTime)
                .Where(x => !IsSurveyCleaned((int)x.Id)).
                Select(x => new CleaningServiceEmailInfo(x))
                .ToList();

            _surveyCleaningConfirmitDataAccess.SetCreators(surveys);

            return surveys;
        }

        /// <summary>
        /// This method return list of surveys which can be cleaned.
        /// </summary>
        /// <returns></returns>
        public List<CleaningServiceEmailInfo> GetSurveysWhichAreReadyForCleanup(DateTime lastTouchTime, DateTime lastSentNoticeTime)
        {
            var surveys = BvSpSurveyCleanup_GetSurveysWhichAreReadyForCleanupAdapter.ExecuteEntityList(lastTouchTime, lastSentNoticeTime)
                .Where(x => !IsSurveyCleaned((int) x.Id))
                .Select(x => new CleaningServiceEmailInfo(x))
                .ToList();

            _surveyCleaningConfirmitDataAccess.SetCreators(surveys);

            return surveys;
        }
        
        public void CleanSurvey(int surveyId)
        {
            DeleteSurveyDataInBatches("BvPersonOrGroupAssignmentOnSurvey", "SurveyId = @SurveyId", surveyId);
            DeleteSurveyDataInBatches("BvPersonRel", "type = 2 AND ObjectSid = @SurveyId", surveyId);
            DeleteSurveyDataInBatches("BvLoginGroup", "SurveySid = @SurveyId", surveyId);
            DeleteSurveyDataInBatches("BvCallHistory", "SurveyId = @SurveyId", surveyId);
            DeleteSurveyDataInBatches("BvCallHistoryEx", "SurveyId = @SurveyId", surveyId);
            DeleteSurveyDataInBatches("BvSvySchedule", "SurveySid = @SurveyId", surveyId);
        }

        private void DeleteSurveyDataInBatches(string tableName, string condition, int surveyId)
        {
            var db = new DatabaseEngine();
            while(true)
            {
                var rowsDeleted = db.ExecuteScalar<int>($@"
                    DELETE TOP({BatchSize}) FROM {tableName} WHERE {condition};
                    SELECT @@ROWCOUNT;", 
                    new SqlParameter("@SurveyId", surveyId));

                if (rowsDeleted < BatchSize)
                    break;
            }
        }

        private bool IsSurveyCleaned(int surveyId)
        {
            BvSpSurveyCleanup_IsCleanAdapter.ExecuteNonQuery(surveyId, out var result);

            return result != 0;
        }
    }
}