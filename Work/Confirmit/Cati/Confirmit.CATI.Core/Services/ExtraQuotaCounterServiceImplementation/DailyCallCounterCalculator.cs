using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Handmade.Entity;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation
{
    class DailyCallCounterCalculator : IExtraQuotaCounterCalculator
    {
        public readonly DailyCounterParameter _parameters;
        public readonly IQuotaInfoService _quotaInfoService;
        public readonly ITimezoneService _timezoneService;
        public readonly ITimeService _timeService;

        public DailyCallCounterCalculator(DailyCounterParameter parameters)
        {
            _parameters = parameters;
            _quotaInfoService = ServiceLocator.Resolve<IQuotaInfoService>();
            _timezoneService = ServiceLocator.Resolve<ITimezoneService>();
            _timeService = ServiceLocator.Resolve<ITimeService>();
        }

        public IEnumerable<QuotaCellCounter> GetCellCounter()
        {
            var startOfToday = GetStartOfTheDayForDefaultCallCenter();
            var query = String.Format(
              @"SELECT {0} as CellDescriptor, COUNT(*) as Counter 
                FROM BvReplicatedData_{1} r
	                INNER JOIN BvHistory h 
                    ON h.InterviewID = r.respid AND h.SurveyID = {1}
                WHERE {2} AND h.firedTime BETWEEN '{3}' AND '{4}' {6}
	            GROUP BY {5} ",
                /*0*/StringService.Join(" + ',' + ", "CAST( r.[{0}] as NVARCHAR(MAX))", _parameters.QuotaFields), //build string like: CAST( r.[q1] AS NVARCHAR(MAX))+ ',' + CAST( r.[q2] AS NVARCHAR(MAX)) 
                /*1*/_parameters.SurveyId,
                /*2*/StringService.Join(" AND ", "r.[{0}] IS NOT NULL", _parameters.QuotaFields),//build string like: r.[q1] IS NOT NULL AND r.[q2] IS NOT NULL AND r.[q3] IS NOT NULL
                /*3*/(_parameters.Period?.startDate ?? startOfToday).ToString("yyyy-MM-dd HH:mm"),
                /*4*/(_parameters.Period?.endDate ?? startOfToday).AddHours(24).ToString("yyyy-MM-dd HH:mm"),
                /*5*/StringService.Join(", ", "r.[{0}]", _parameters.QuotaFields),//build string like: r.[q1], r.[q2], r.[q3]
                /*6*/_parameters.Its.Length > 0 ? String.Format("AND h.Its IN ({0})", String.Join(",", _parameters.Its)) : String.Empty
                );

            return ExtraQuotaCounterService.ExecuteExtraCellCounterQuery(query);
        }

        public IEnumerable<KeyValuePair<int, int>> GetItsCountersForCell(int cellId)
        {
            var startOfToday = GetStartOfTheDayForDefaultCallCenter();
            var cellValues = _quotaInfoService.GetCellValues(_parameters.SurveyId, _parameters.QuotaId, cellId, _parameters.QuotaFields);

            var filterBySpecificIts = _parameters.Its.Length > 0 ? String.Format("AND h.Its IN ({0})", String.Join(",", _parameters.Its)) : String.Empty ;

            var query = String.Format(
                @"SELECT CAST(h.ITS AS INT) as ITS, COUNT(*) as Counter
	              FROM BvHistory h
                       INNER JOIN BvReplicatedData_{0} rd
	                   ON h.SurveyID = {0} AND h.InterviewID = rd.respid
                  WHERE h.firedTime BETWEEN '{1}' AND '{2}' AND {3} {4}      
                  GROUP BY h.ITS",                                                 
                  /*0*/_parameters.SurveyId,
                /*1*/startOfToday.ToString("yyyy-MM-dd HH:mm"),
                /*2*/startOfToday.AddHours(24).ToString("yyyy-MM-dd HH:mm"),
                  /*3*/QuotaService.GetCellWhereForRepicationTable("rd", _parameters.QuotaFields, cellValues),
                  /*4*/ filterBySpecificIts); //where clause to filter only records for a specific cell

            return ExtraQuotaCounterService.ExecuteExtraItsCellCounterQuery(query);
        }

        public int GetTotalCounter()
        {
            return 0;
        }

        public string GetFormatedTotalCounter()
        {
            return "Achieved counters for interviews with selected statuses";
        }

        private DateTime GetStartOfTheDayForDefaultCallCenter()
        {
            var defaultTimezoneId = _timezoneService.GetDefaultCallCenterTimezoneId();
            var startDateOfCallCenterLocalTime = _timezoneService.ConvertTimeFromUtc(defaultTimezoneId, _timeService.GetUtcNow()).Date;
            return (_timezoneService.ConvertTimeToUtc(defaultTimezoneId, startDateOfCallCenterLocalTime)); 
        }

    }
}
