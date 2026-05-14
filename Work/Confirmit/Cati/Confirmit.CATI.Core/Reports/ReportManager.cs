using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Paging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Timezones;

using Enumerable = System.Linq.Enumerable;
using System.Data;
using Confirmit.CATI.Core.Reports.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Reports
{
    //TODO: Refactor and use adapters.

    /// <summary>
	/// Class that is responsible for common operations/data extractions for reports
	/// </summary>
	public static class ReportManager
	{
        /// <summary>
        /// Gets data for sample status summary report
        /// </summary>
        /// <param name="surveySid">Survey's SID</param>
        /// <param name="persons">String containing comma-delimited SIDs of persons (like "1,2,3,7")</param>
        /// <param name="itsIds">String containing comma-delimited IDs of ITS (like "1,2,3,7")</param>
        /// <param name="hideZero"> </param>
        /// <returns>List of sample status summary report's records</returns>
        public static List<SampleStatusSummaryRecord> GetSssData(int surveySid, IEnumerable<int> persons, string itsIds, bool hideZero)
	    {
	        var personsSids = ConvertArrayToStringParameter(persons);

            var evt = new BuildSurveySummaryReportEvent(surveySid, personsSids, itsIds);

            List<SampleStatusSummaryRecord> result = 
                Enumerable.ToList(BvSpReportSampleStatusSummaryAdapter.ExecuteEntityList(
                     surveySid, 
                     personsSids, 
                     itsIds,
                     hideZero
                     ).Select(entity => new SampleStatusSummaryRecord(entity)));

		    evt.Finish();

			return result;
		}

        /// <summary>
        /// Returns 'recordset' for the productivity report with the specified filtering parameters
        /// </summary>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <param name="surveys">List of survey sids to filter by</param>
        /// <param name="persons">List of person sids to filter by (null if no filter)</param>
        /// <param name="its">List of ITS ids to filter by (null if no filter)</param>
        /// <param name="surveyDataFilter">Survey data filter.</param>
        /// <param name="startShiftTime">Start of shift time.</param>
        /// <param name="endShiftTime">End of shift time.</param>
        /// <param name="callCenterId">Id of the call center. Use null for all call centers.</param>
        /// <returns></returns>
        /// <remarks>See unit test for this method in FusionLib.UnitTests.ReportManagerTest.</remarks>
        public static List<ProductivityReportRecord> GetProductivityReportData(
                DateTime startDate,
                DateTime endDate,
                IEnumerable<int> surveys,
                IEnumerable<int> persons,
                IEnumerable<string> its,
                string surveyDataFilter,
                DateTime? startShiftTime,
                DateTime? endShiftTime,
                int? callCenterId = null
            )
        {
            var evt = new BuildSurveyProductivityReportEvent(startDate, endDate, surveys, its, persons);

            List<ProductivityReportRecord> result;

            result = Enumerable.ToList(BvSpSurveyProductivityReportAdapter.ExecuteEntityList(
                    ConvertArrayToStringParameter(surveys),
                    ConvertArrayToStringParameter(persons),
                    ConvertArrayToStringParameter(its),
                    startDate,
                    endDate,
                    surveyDataFilter, startShiftTime, endShiftTime, callCenterId).Select(entity => new ProductivityReportRecord(entity)));
            evt.Finish();

            return result;
        }

        /// <summary>
        /// Gets the data for interviewer productivity report.
        /// </summary>
        /// <param name="surveys">Survey SID to get report data for.</param>
        /// <param name="startDate">Start time of the interval to get report data for.</param>
        /// <param name="endDate">End time of the interval to get report data for.</param>
        /// <param name="persons">List of person IDs to get report data for.</param>
        /// <param name="states">List of extended statuses to get report data for.</param>
        /// <param name="showDialerAttempts">Do we need to include disconnected dialer attempts row in the report.
        /// Useful for predictive dialing mode only.</param>
        /// <param name="hideEmpty">Do we need to exclude interviewers with 0 log on time.</param>
        /// <param name="surveyDataFilter">Survey data filter.</param>
        /// <param name="startShiftTime">Start of shift time.</param>
        /// <param name="endShiftTime">End of shift time.</param>
        /// <returns>List of InterviewerProductivityReportItem objects with report data.</returns>
        public static List<InterviewerProductivityReportItem> GetInterviewerProductivityReportData(
            IEnumerable<int> surveys,
            DateTime startDate,
            DateTime endDate,
            IEnumerable<int> persons,
            IEnumerable<int> states,
            bool showDialerAttempts,
            bool hideEmpty,
            bool calcAllBreakHistory,
            string surveyDataFilter,
            DateTime? startShiftTime,
            DateTime? endShiftTime)
        {
            var evt = new BuildInterviewerProductivityReportEvent(
                surveys, 
                startDate, 
                endDate, 
                states,
                persons,
                showDialerAttempts,
                hideEmpty);

            var result = Enumerable.ToList(BvSpInterviewerProductivityReportAdapter.ExecuteEntityList(
                ConvertArrayToStringParameter(surveys), 
                ConvertArrayToStringParameter(persons), 
                ConvertArrayToStringParameter(states), 
                showDialerAttempts, 
                hideEmpty, 
                calcAllBreakHistory,
                startDate, endDate, surveyDataFilter, startShiftTime, endShiftTime
                ).Select(
                    entity => new InterviewerProductivityReportItem
                        {
                            PersonId = entity.PersonId.Value, 
                            PersonName = entity.PersonName, 
                            Completes = entity.Completes.Value, 
                            LogOnTime = entity.LogOnTime.Value, 
                            WaitingTime = entity.WaitingTime.Value, 
                            BreakTimePaid = entity.OnBreakTimePaid.Value,
                            BreakTimeUnpaid = entity.OnBreakTimeUnpaid.Value,
                            DialigsCount = entity.DialingsCount.Value, 
                            AverageCompletedInterviewDuration = entity.AverageCompletedInterviewDuration.Value,
                            OpenEndReviewDuration = entity.OpenEndReviewDuration.Value
                        }
                ));

            evt.Finish();

            return result;
        }

	    /// <summary>
	    /// Gets the data for survey overview report.
	    /// </summary>
	    /// <param name="surveys">Survey SIDs to get report data for.</param>
	    /// <param name="startDate">Start time of the interval to get report data for.</param>
	    /// <param name="endDate">End time of the interval to get report data for.</param>
	    /// <param name="persons">List of person IDs to get report data for.</param>
	    /// <param name="states">List of extended statuses to get report data for.</param>
	    /// <param name="showDialerAttempts">Do we need to include disconnected dialer attempts row in the report.
	    ///     Useful for predictive dialing mode only.</param>
	    /// <param name="hideEmpty">Do we need to exclude interviewers with 0 log on time.</param>
        /// <param name="surveyDataFilter">Survey data filter.</param>
        /// <param name="startShiftTime">Start of shift time.</param>
        /// <param name="endShiftTime">End of shift time.</param>
        /// <returns>List of <see cref="SurveyOverviewReportItem"/> objects with report data.</returns>
	    public static List<SurveyOverviewReportItem> GetSurveyOverviewReportData(
            IEnumerable<int> surveys, 
            DateTime startDate, 
            DateTime endDate, 
            IEnumerable<int> persons, 
            IEnumerable<int> states, 
            bool showDialerAttempts, 
            bool hideEmpty,
            string surveyDataFilter,
            DateTime? startShiftTime,
            DateTime? endShiftTime,
            int? callCenterId = null)
        {
            var evt = new BuildSurveyOverviewReportEvent(
                surveys,
                startDate,
                endDate,
                states,
                persons,
                showDialerAttempts,
                hideEmpty);

	        var surveysString = ConvertArrayToStringParameter(surveys);
            var personsString = ConvertArrayToStringParameter(persons);
            var statesString = ConvertArrayToStringParameter(states);

            var entities = BvSpSurveyOverviewReportAdapter.ExecuteEntityList(
                surveysString,
                personsString,
                statesString,
                showDialerAttempts,
                hideEmpty,
                startDate,
                endDate,
                surveyDataFilter,
                startShiftTime,
                endShiftTime,
                ServiceLocator.Resolve<ISystemSettings>().Console.IncludeOpenEndReviewTimeInInterviewDuration,
                callCenterId);

            var result =
                from entity in entities
                select new SurveyOverviewReportItem
                {
                    ProjectId = entity.ProjectId,
                    ProjectName = entity.Title,
                    Completes = entity.Completes.GetValueOrDefault(),
                    LogOnTime = entity.LogOnTime.GetValueOrDefault(),
                    WaitingTime = entity.WaitingTime.GetValueOrDefault(),
                    DialigsCount = entity.DialingsCount.GetValueOrDefault(),
                    AverageCompletedInterviewDuration = entity.AverageCompletedInterviewDuration.GetValueOrDefault()
                };

            evt.Finish();

            return result.ToList();
        }

	    public static string ConvertArrayToStringParameter<T>(IEnumerable<T> array)
	    {
	        if( array == null || !array.Any()) 
                return null;
            return String.Join(",", array.Select(x => x.ToString()).ToArray());
	    }

	    /// <summary>
        /// Gets the data for attempts by disposition report.
        /// </summary>
        /// <param name="surveySid">Survey SID to get report data for.</param>
        /// <param name="itsIds">List of ITS (extended statuses) to get report data for.</param>
        /// <param name="hideEmpty">Do we need to hide empty rows (where records count for all attempts is equal to 0).</param>
        /// <param name="startDate">Start time of the interval to get report data for.</param>
        /// <param name="endDate">End time of the interval to get report data for.</param>
        /// <returns>List of AttemptsByDispositionReportItem objects with report data.</returns>
        public static List<AttemptsByDispositionReportItem> GetAttemptsByDispositionReportData(
            int surveySid,
            IEnumerable<int> itsIds,
            bool hideEmpty,
            DateTime startDate,
            DateTime endDate,
            int? callCenterId = null
            )
        {
            var evt = new BuildAttemptsByDispositionReportEvent(
               surveySid,
               startDate,
               endDate,
               itsIds,
               hideEmpty);

            var result = BvSpAttemptsByDispositionReportAdapter.ExecuteEntityList(
                surveySid, 
                ConvertArrayToStringParameter(itsIds), 
                hideEmpty, 
                startDate, 
                endDate,
                callCenterId
            ).Select(
                entity => new AttemptsByDispositionReportItem
                {
                    Code = entity.Code.Value, 
                    Disposition = entity.Disposition, 
                    Attempts1 = entity.Attempts1.Value, 
                    Attempts2 = entity.Attempts2.Value, 
                    Attempts3 = entity.Attempts3.Value, 
                    Attempts4 = entity.Attempts4.Value, 
                    Attempts5 = entity.Attempts5.Value, 
                    Attempts6 = entity.Attempts6.Value, 
                    Attempts7 = entity.Attempts7.Value, 
                    Attempts8 = entity.Attempts8.Value, 
                    Attempts9 = entity.Attempts9.Value, 
                    Attempts10 = entity.Attempts10.Value,
                    Attempts11AndMore = entity.Attempts11AndMore.Value
               }
           ).ToList();

            evt.Finish();

            return result;
        }

        /// <summary>
        /// Returns page of call attempts report data. Event date is returned in site timezone.
        /// </summary>
        /// <param name="supervisorName">Supervisor name.</param>
        /// <param name="timezoneId">Timezone ID of dates in search conditions.</param>
        /// <param name="pagingArgs">Paging arguments.</param>
        /// <param name="includeDisposedByDialerAttempts">Show or not attempts disposed by dialer</param>
        /// <param name="hidePii">Hide PII</param>
        /// <param name="totalCount">Returns total count of records.</param>
        /// <returns>Reports page.</returns>
        public static List<CallAttemptsReportRecord> GetCallAttemptsPage(
            string supervisorName,
            int timezoneId,
            PagingArgs pagingArgs,
            bool includeDisposedByDialerAttempts,
            bool hidePii,
            out int totalCount
        )
        {
            var evt = new BuildCallAttemptLogEvent(supervisorName, timezoneId, pagingArgs);

            var result = new List<CallAttemptsReportRecord>();
            var data = BvSpGetCallAttemptsReport_ListPageAdapter.ExecuteEntityList(
                    supervisorName,
                    pagingArgs.PageIndex,
                    pagingArgs.PageSize,
                    pagingArgs.SortField,
                    pagingArgs.SortOrderAsc ? 1 : 0,
                    SearchManager.GetSqlCondition(pagingArgs.SearchParameters, timezoneId),
                    includeDisposedByDialerAttempts,
                    out totalCount);

            foreach (var entity in data)
            {
                result.Add(new CallAttemptsReportRecord(entity, timezoneId, hidePii));
            }

            evt.Finish();

            return result;
        }

        public static List<BvSpGetInboundCallsReport_ListPageEntity> GetInboundCallsReportPage(string supervisorName, int timezoneId, PagingArgs pagingArgs, out int totalCount)
        {
            var evt = new BuildInboundCallsReportEvent(supervisorName, timezoneId, pagingArgs);

            var data = BvSpGetInboundCallsReport_ListPageAdapter.ExecuteEntityList(
                    supervisorName,
                    pagingArgs.PageIndex,
                    pagingArgs.PageSize,
                    pagingArgs.SortField,
                    pagingArgs.SortOrderAsc ? 1 : 0,
                    SearchManager.GetSqlCondition(pagingArgs.SearchParameters, timezoneId),
                    out totalCount).ToList();

            evt.Finish();

            return data;
        }
        
        /// <summary>
        /// Gets the data for number of attempts report.
        /// </summary>
        /// <param name="surveySid">Survey SID to get report data for.</param>
        /// <param name="startDate">Start time of the interval to get report data for.</param>
        /// <param name="endDate">End time of the interval to get report data for.</param>
        /// <param name="sampleSize">Total sample size for survey.</param>
        /// <returns>List of NumberOfAttemptsReportItem objects with report data.</returns>
        public static List<NumberOfAttemptsReportItem> GetNumberOfAttemptsReportData( int surveySid, DateTime startDate, DateTime endDate, int? callCenterId, out int sampleSize)
        {
            var evt = new BuildNumberOfAttemptsReportEvent(
                surveySid,
                startDate,
                endDate);

            var result = BvSpNumberOfAttemptsReportAdapter.ExecuteEntityList(
                surveySid, 
                startDate, 
                endDate, 
                callCenterId,
                out sampleSize
            ).Select(
                entity => new NumberOfAttemptsReportItem
                {
                    Attempts = entity.Attempts.Value, 
                    Records = entity.Records.Value
                }
            ).ToList();

            evt.Finish();

            return result;
        }

        public static List<AlertsHistoryAggregatedReportItem> GetAggregatedAlertsHistory(
            IEnumerable<int> surveys,
            IEnumerable<int> persons,
            DateTime startDate,
            DateTime endDate,
            InterviewerSubmissionAlert alert,
            byte? interviewStateFilter)
        {
            var evt = new BuildAggregatedAlertsHistoryReportEvent(
                persons.ToArray(), surveys.ToArray(), startDate, endDate, interviewStateFilter);

            var list = BvSpAlertsHistoryAggregatedReportAdapter.ExecuteEntityList(ConvertArrayToStringParameter(persons),
                                                                                  ConvertArrayToStringParameter(surveys),
                                                                                  startDate,
                                                                                  endDate,
                                                                                  interviewStateFilter);

            var result = ProcessAggregatedAlertsHistoryData(list, alert);

            evt.Finish();

            return result;
        }

	    public static List<AlertsHistoryAggregatedReportItem> ProcessAggregatedAlertsHistoryData(
            List<BvSpAlertsHistoryAggregatedReportEntity> data,
            InterviewerSubmissionAlert alert)
        {
            switch (alert)
            {
                case InterviewerSubmissionAlert.All:
                    return (from item in data
                            select new AlertsHistoryAggregatedReportItem
                                {
                                    InterviewerId = item.PersonId.GetValueOrDefault(),
                                    InterviewerName = item.PersonName,
                                    AmberCount =
                                        item.AnswerSubmissionAmberCounts.GetValueOrDefault() +
                                        item.QuickAnswerSubmissionAmberCounts.GetValueOrDefault(),
                                    RedCount =
                                        item.AnswerSubmissionRedCounts.GetValueOrDefault() +
                                        item.QuickAnswerSubmissionRedCounts.GetValueOrDefault(),
                                    TotalCount =
                                        item.AnswerSubmissionAmberCounts.GetValueOrDefault() +
                                        item.QuickAnswerSubmissionAmberCounts.GetValueOrDefault() +
                                        item.AnswerSubmissionRedCounts.GetValueOrDefault() +
                                        item.QuickAnswerSubmissionRedCounts.GetValueOrDefault(),
                                }).ToList();
                case InterviewerSubmissionAlert.LastSubmission:
                    return (from item in data
                            select new AlertsHistoryAggregatedReportItem
                                {
                                    InterviewerId = item.PersonId.GetValueOrDefault(),
                                    InterviewerName = item.PersonName,
                                    AmberCount = item.AnswerSubmissionAmberCounts.GetValueOrDefault(),
                                    RedCount = item.AnswerSubmissionRedCounts.GetValueOrDefault(),
                                    TotalCount =
                                        item.AnswerSubmissionAmberCounts.GetValueOrDefault() +
                                        item.AnswerSubmissionRedCounts.GetValueOrDefault()

                                }).ToList();
                case InterviewerSubmissionAlert.QuickAnswer:
                    return (from item in data
                            select new AlertsHistoryAggregatedReportItem
                                {
                                    InterviewerId = item.PersonId.GetValueOrDefault(),
                                    InterviewerName = item.PersonName,
                                    AmberCount = item.QuickAnswerSubmissionAmberCounts.GetValueOrDefault(),
                                    RedCount = item.QuickAnswerSubmissionRedCounts.GetValueOrDefault(),
                                    TotalCount =
                                        item.QuickAnswerSubmissionAmberCounts.GetValueOrDefault() +
                                        item.QuickAnswerSubmissionRedCounts.GetValueOrDefault()
                                }).ToList();
                default:
                    throw new ArgumentOutOfRangeException("alert");
            }
        }

        public static List<BvSpAlertsHistoryReportEntity> GetAlertsHistory(
            IEnumerable<int> surveys, IEnumerable<int> persons, PagingArgs pagingArgs, int timezoneId, out int totalCount)
        {
            var evt = new BuildAlertsHistoryReportEvent(persons.ToArray(), surveys.ToArray(), pagingArgs);

            var list = BvSpAlertsHistoryReportAdapter.ExecuteEntityList(
                ConvertArrayToStringParameter(persons),
                ConvertArrayToStringParameter(surveys),
                SearchManager.GetSqlCondition(pagingArgs.SearchParameters, timezoneId),
                pagingArgs.PageIndex,
                pagingArgs.PageSize,
                pagingArgs.SortField,
                pagingArgs.SortOrderAsc,
                out totalCount);

            list.ForEach(x => x.SubmissionTime = TimezoneManager.ConvertToTzLocalTime(timezoneId, x.SubmissionTime.GetValueOrDefault()));

            evt.Finish();

            return list;
        }

        public static List<InterviewerSessionsReportEntity> GetInterviewerSessions(InterviewerSessionsReportParams parameters, out int totalCount)
        {
            var evt = new BuildInterviewerSessionsReportEvent(parameters.Persons.ToArray(), parameters.PagingArgs);

            var interviewerSessionsReportQuery = ServiceLocator.Resolve<IInterviewerSessionsReportQuery>();
            List<InterviewerSessionsReportEntity> list = interviewerSessionsReportQuery.Execute(parameters, out totalCount);

            evt.Finish();

            return list;
        }
	}
}
