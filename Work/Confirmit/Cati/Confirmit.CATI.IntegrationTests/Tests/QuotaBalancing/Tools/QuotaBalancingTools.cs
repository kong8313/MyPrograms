using System;
using System.Linq;

using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using System.Data.SqlClient;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.IntegrationTests.Tests.QuotaBalancing.Tools
{
    public class QuotaBalancingTools
    {
        static public void SetProcessedCallsPerMinute(int processedCallsPerMinute, int completedCallsPerMinute, int surveyId)
        {
            var aggregateSurvey = BvAggregateSurveyAlertStatusAdapter.GetByCondition("SID = @surveyId", new SqlParameter("@surveyId", surveyId)).First();
            aggregateSurvey.CountCalls = processedCallsPerMinute * 60;
            aggregateSurvey.StrikeRate = completedCallsPerMinute * 60;
            BvAggregateSurveyAlertStatusAdapter.Update(aggregateSurvey);
        }

        static public void SetSleepPeriod(int sleepPeriodInMinutes)
        {
            ServiceLocator.Resolve<ISystemSettings>().QuotaBalancing.MinDelay = TimeSpan.FromMinutes(sleepPeriodInMinutes);
        }
    }
}
