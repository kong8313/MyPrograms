using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Core.DAL.Handmade.Entity;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    internal class InterviewsCounterCalculator : IExtraQuotaCounterCalculator
    {
        public InterviewsCounterParameter Parameters { get; set; }
        public readonly IQuotaInfoService _quotaInfoService;

        public InterviewsCounterCalculator(InterviewsCounterParameter parameters)
        {
            Parameters = parameters;
            _quotaInfoService = ServiceLocator.Resolve<IQuotaInfoService>();
        }

        public IEnumerable<QuotaCellCounter> GetCellCounter()
        {
            if (Parameters.Its.Length <= 0)
            {
                return new QuotaCellCounter[] { };
            }

            var query = String.Format(
                @"  SELECT {0} as CellDescriptor, COUNT(*) as Counter FROM BvReplicatedData_{1} r
	                INNER JOIN BvInterview i
	                ON r.respId = i.ID AND i.SurveySID = {1}
	                WHERE   {2} AND i.TransientState IN ( {3} )
	                GROUP BY {4}",
                StringService.Join(" + ',' + ", "CAST( r.[{0}] as NVARCHAR(MAX))", Parameters.QuotaFields),//build string like: CAST( r.[q1] AS NVARCHAR(MAX))+ ',' + CAST( r.[q2] AS NVARCHAR(MAX)) 
                Parameters.SurveyId,
                StringService.Join(" AND ", "r.[{0}] IS NOT NULL", Parameters.QuotaFields),//build string like: r.[q1] IS NOT NULL AND r.[q2] IS NOT NULL AND r.[q3] IS NOT NULL
                String.Join(",", Parameters.Its.Select(x => x.ToString()).ToArray()),
                StringService.Join(", ", "r.[{0}]", Parameters.QuotaFields)//build string like: r.[q1], r.[q2], r.[q3]
                );

            return ExtraQuotaCounterService.ExecuteExtraCellCounterQuery(query);

        }

        public IEnumerable<KeyValuePair<int, int>> GetItsCountersForCell(int cellId)
        {
            var cellValues = _quotaInfoService.GetCellValues(Parameters.SurveyId, Parameters.QuotaId, cellId,
                                            Parameters.QuotaFields);

            if (Parameters.Its == null || Parameters.Its.Length <= 0)
                return new KeyValuePair<int, int>[0];

            var query =
                String.Format(
                    @"  SELECT i.TransientState as ITS, COUNT(*) as Counter
	                    FROM BvInterview i
	                    INNER JOIN BvReplicatedData_{0} rd
	                    ON i.SurveySID = {0} AND i.ID = rd.respid
	                    WHERE {1} AND i.TransientState IN ({2})
	                    GROUP BY i.TransientState",
                    Parameters.SurveyId,
                    QuotaService.GetCellWhereForRepicationTable("rd", Parameters.QuotaFields, cellValues),
                    String.Join(",", Parameters.Its));

            return ExtraQuotaCounterService.ExecuteExtraItsCellCounterQuery(query);
        }

        public int GetTotalCounter()
        {
            return InterviewService.GetCountOfInterviewsWithSpecificITSs(Parameters.SurveyId, Parameters.Its);
        }

        public string GetFormatedTotalCounter()
        {
            return String.Format("Total interviews with specific statuses: {0}", GetTotalCounter());
        }
    }
}
