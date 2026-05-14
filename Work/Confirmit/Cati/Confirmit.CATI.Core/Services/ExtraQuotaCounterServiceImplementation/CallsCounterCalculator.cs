using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Handmade.Entity;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    class CallsCounterCalculator : IExtraQuotaCounterCalculator
    {
        public CallsCounterParameter Parameters { get; private set; }
        public readonly IQuotaInfoService _quotaInfoService;

        public CallsCounterCalculator( CallsCounterParameter parameters )
        {
            this.Parameters = parameters;
            _quotaInfoService = ServiceLocator.Resolve<IQuotaInfoService>();
        }

        #region IExtraQuotaCounterCalculator Members

        public IEnumerable<QuotaCellCounter> GetCellCounter()
        {
            if (this.Parameters.Its == null)
            {
                return this.GetCellCounterOfScheduledInterviewsWithAllITS();
            }
            
            return this.GetCellCounterOfScheduledInterviewsWithSpecificITS();
        }

        private IEnumerable<QuotaCellCounter> GetCellCounterOfScheduledInterviewsWithAllITS()
        {
            var callStates = GetAvailableCallStatesForScheduledCalls(this.Parameters.IncludeDisabledCalls);

            var query = String.Format(
                @"  SELECT {0} as CellDescriptor, COUNT(*) as Counter FROM BvReplicatedData_{1} r
	                INNER JOIN BvSvySchedule c
    	            ON r.respId = c.InterviewID AND c.SurveySID = {1}
	                WHERE {2} AND ( c.CallState IN ({4}) )
	                GROUP BY {3}",
                StringService.Join(" + ',' + ", "CAST( r.[{0}] as NVARCHAR(MAX))", this.Parameters.QuotaFields),//build string like: CAST( r.[q1] AS NVARCHAR(MAX))+ ',' + CAST( r.[q2] AS NVARCHAR(MAX)) 
                this.Parameters.SurveyId,
                StringService.Join(" AND ", "r.[{0}] IS NOT NULL", this.Parameters.QuotaFields),//build string like: r.[q1] IS NOT NULL AND r.[q2] IS NOT NULL AND r.[q3] IS NOT NULL
                StringService.Join(", ", "r.[{0}]", this.Parameters.QuotaFields),//build string like: r.[q1], r.[q2], r.[q3]
                String.Join<int>(",", callStates)
                );

            return ExtraQuotaCounterService.ExecuteExtraCellCounterQuery(query);
        }

        private static IEnumerable<int> GetAvailableCallStatesForScheduledCalls(bool includeDisabledCalls)
        {
            yield return (int) CallState.LoadedToDialerPredictively;
            yield return (int) CallState.Scheduled;

            if (!includeDisabledCalls) yield break;

            yield return (int)CallState.DisabledByFCD;
            yield return (int)CallState.DisabledByUser;
        }

        private IEnumerable<QuotaCellCounter> GetCellCounterOfScheduledInterviewsWithSpecificITS()
        {
            if (this.Parameters.Its == null || this.Parameters.Its.Count() <= 0)
            {
                return new QuotaCellCounter[] { };
            }

            var callStates = GetAvailableCallStatesForScheduledCalls(this.Parameters.IncludeDisabledCalls);

            var query = String.Format(
                @"  SELECT {0} as CellDescriptor, COUNT(*) as Counter FROM BvReplicatedData_{1} r
                    INNER JOIN BvSvySchedule c
    	            ON r.respId = c.InterviewID AND c.SurveySID = {1}
	                INNER JOIN BvInterview i
	                ON r.respId = i.ID AND i.SurveySID = {1}
	                WHERE   {2} AND ( c.CallState IN ({5}) ) AND i.TransientState IN ( {3} )
	                GROUP BY {4}",
                StringService.Join(" + ',' + ", "CAST( r.[{0}] as NVARCHAR(MAX))", this.Parameters.QuotaFields),//build string like: CAST( r.[q1] AS NVARCHAR(MAX))+ ',' + CAST( r.[q2] AS NVARCHAR(MAX)) 
                this.Parameters.SurveyId,
                StringService.Join(" AND ", "r.[{0}] IS NOT NULL", this.Parameters.QuotaFields),//build string like: r.[q1] IS NOT NULL AND r.[q2] IS NOT NULL AND r.[q3] IS NOT NULL
                String.Join(",", this.Parameters.Its.Select(x => x.ToString()).ToArray()),
                StringService.Join(", ", "r.[{0}]", this.Parameters.QuotaFields),//build string like: r.[q1], r.[q2], r.[q3]
                String.Join<int>(",", callStates)
                );

            return ExtraQuotaCounterService.ExecuteExtraCellCounterQuery(query);
        }

        public IEnumerable<KeyValuePair<int, int>> GetItsCountersForCell(int cellId)
        {
            var cellValues = _quotaInfoService.GetCellValues(Parameters.SurveyId, Parameters.QuotaId, cellId,
                                                        Parameters.QuotaFields);

            var callStates = GetAvailableCallStatesForScheduledCalls(Parameters.IncludeDisabledCalls);

            var additianalWhereCondition = Parameters.Its == null
                                               ? ""
                                               : String.Format(
                                                   "AND i.TransientState IN ({0})", String.Join(",", Parameters.Its));

            var query =
                String.Format(
                    @"  SELECT i.TransientState as ITS, COUNT(*) as Counter
	                    FROM BvInterview i
	                    INNER JOIN BvReplicatedData_{0} rd
	                    ON i.SurveySID = {0} AND i.ID = rd.respid
	                    INNER JOIN BvSvySchedule c 
	                    ON i.SurveySID = c.SurveySID AND i.ID = c.InterviewID
	                    WHERE c.CallState IN ({1}) AND {2} {3}
                        GROUP BY i.TransientState",
                    Parameters.SurveyId,
                    String.Join(",", callStates),
                    QuotaService.GetCellWhereForRepicationTable("rd", Parameters.QuotaFields, cellValues),
                    additianalWhereCondition);

            return ExtraQuotaCounterService.ExecuteExtraItsCellCounterQuery(query);
        }

        public int GetTotalCounter()
        {
            if( Parameters.Its != null )
                return CallQueueService.GetCountOfScheduledInterviewsWithSpecificIts(Parameters.SurveyId, Parameters.Its);

            return CallQueueService.GetCountOfScheduledInterviews(Parameters.SurveyId);
        }

        public string GetFormatedTotalCounter()
        {
            if (Parameters.Its != null)
                return String.Format("Total scheduled interviews with specific statuses: {0}", CallQueueService.GetCountOfScheduledInterviewsWithSpecificIts(Parameters.SurveyId, Parameters.Its));

            return String.Format("Total scheduled interviews: {0}", CallQueueService.GetCountOfScheduledInterviews(Parameters.SurveyId));
        }

        #endregion
    }
}
