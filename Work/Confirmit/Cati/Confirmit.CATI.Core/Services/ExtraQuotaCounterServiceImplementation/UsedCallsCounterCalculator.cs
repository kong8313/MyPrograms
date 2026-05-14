using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.DAL.Handmade.Entity;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    public class UsedCallsCalculator : IUsedCallsCalculator
    {
        public IEnumerable<QuotaCellCounter> GetCountersOfNotScheduledExcludingCompletes(IExtraQuotaCounterParameters parameters)
        {
            var query = String.Format(
                @"  SELECT {0} as CellDescriptor, COUNT(*) as Counter FROM BvReplicatedData_{1} r
	                INNER JOIN Bvinterview i
                    ON i.ID = r.respid AND i.SurveySID = {1}
	                LEFT JOIN BvSvySchedule c
    	            ON r.respId = c.InterviewID AND c.SurveySID = {1}
                    WHERE {2} AND c.ID IS NULL AND i.TransientState <> 13
	                GROUP BY {3}",
                StringService.Join(" + ',' + ", "CAST( r.[{0}] as NVARCHAR(MAX))", parameters.QuotaFields),//build string like: CAST( r.[q1] AS NVARCHAR(MAX))+ ',' + CAST( r.[q2] AS NVARCHAR(MAX)) 
                parameters.SurveyId,
                StringService.Join(" AND ", "r.[{0}] IS NOT NULL", parameters.QuotaFields),//build string like: r.[q1] IS NOT NULL AND r.[q2] IS NOT NULL AND r.[q3] IS NOT NULL
                StringService.Join(", ", "r.[{0}]", parameters.QuotaFields)//build string like: r.[q1], r.[q2], r.[q3]
                );

            return ExtraQuotaCounterService.ExecuteExtraCellCounterQuery(query);
        }
    }
}
