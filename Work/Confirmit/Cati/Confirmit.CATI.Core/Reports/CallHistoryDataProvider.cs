using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Cache;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Adapter.Query;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Reports
{
    public class CallHistoryDataProvider : ICallHistoryDataProvider
    {
        private readonly IPersonSessionHistoryRepository _sessionHistoryRepository;

        public CallHistoryDataProvider()
        {
            _sessionHistoryRepository = ServiceLocator.Resolve<IPersonSessionHistoryRepository>();
        }

        public bool IncludeReplicatedVariables { get; set; }

        public object[] PrepareForExport(CallHistoryDataEntity x)
        {
            var obj = new List<object>
            {
                x.FiredTime,
                x.ProjectID,
                x.Name,
                x.InterviewID,
                x.InterviewerID,
                x.InterviewerName ?? "DELETED INTERVIEWER",
                x.TelephoneNumber,
                x.ExtendedStatus,
                x.Duration,
                x.WaitingTime
            };

            if (BackendInstance.Current.HasCallCentersAddon)
            {
                obj.Add(x.CallCenterName);
            }

            if (IncludeReplicatedVariables && x.ReplicatedVariables != null)
            {
                obj.Add(string.Join("\t", x.ReplicatedVariables));
            }

            return obj.ToArray();
        }

        public string GetHeader(string replicatedVariables)
        {
            var header = Strings.ExportCallHistoryGeneralColumns;

            if (BackendInstance.Current.HasCallCentersAddon)
            {
                header += string.Format(";{0}", Strings.ExportCallHistoryCallCenterColumn);
            }

            if (IncludeReplicatedVariables)
            {
                header += string.Format(";{0}", replicatedVariables);
            }

            return string.Join("\t", new DelimitedStringCleaner().ParseString(header)) + Environment.NewLine;
        }

        public IEnumerable<CallHistoryDataEntity> GetPersonSessionHistoryData(int? callCenterId, DateTime? startTime, DateTime? finishTime)
        {
            var events = _sessionHistoryRepository.GetSessionEvents(callCenterId, BackendInstance.Current.CompanyId, startTime, finishTime);
            return GetCallHistoryEntities(events);
        }

        public IEnumerable<CallHistoryDataEntity> GetInterviewerBreaksData(string surveySIDs, DateTime? startTime, DateTime? endTime)
        {
            var interviewerTimeBreaksList = TimeBreaksHistoryService.GetInterviewerBreaks(surveySIDs, startTime, endTime);
            return GetCallHistoryEntities(interviewerTimeBreaksList);
        }

        public List<CallHistoryDataEntity> GetCallHistoryData(string surveySIDs, DateTime? startTime, DateTime? endTime, string[] replicatedVariables)
        {
            var maxRows = ServiceLocator.Resolve<ISystemSettings>().Reports.CallHistoryReportCallHistoryRowsLimit;

            var surveyIdsDelimited = new DelimitedStringCleaner().ParseString(surveySIDs);

            var surveys =
                BvSurveyCache.Instance.GetAll()
                    .Where(
                        x =>
                            (surveySIDs == null || surveyIdsDelimited.Any(y => y.Equals(x.SID.ToString(CultureInfo.InvariantCulture)))) &&
                            (x.State == (int)SurveyState.Open || x.State == (int)SurveyState.Close))
                    .OrderBy(x => x.SID);
            var callHistoryList = new List<CallHistoryDataEntity>();
            var adapter = new CallHistoryDataAdapter();

            foreach (var survey in surveys)
            {
                var received = adapter.GetForSurvey(survey.SID, startTime, endTime, replicatedVariables, maxRows).ToList();
                callHistoryList.AddRange(received);
                maxRows -= received.Count;
                if (maxRows <= 0)
                    break;
            }

            return callHistoryList;
        }

        public List<CallHistoryDataEntity> GetCallHistoryData(
            string surveyIds, DateTime? startTime, DateTime? endTime, string[] variables, bool includeBreakTimes, bool includeLoginLogoutInfo)
        {
            var callHistoryList = GetCallHistoryData(surveyIds, startTime, endTime, variables);

            if (includeBreakTimes)
            {
                var interviewerTimeBreaksEntities = GetInterviewerBreaksData(surveyIds, startTime, endTime);

                callHistoryList = callHistoryList.Concat(interviewerTimeBreaksEntities).ToList();
            }

            if (includeLoginLogoutInfo)
            {
                var bvSpCallHistoryDataEntities = GetPersonSessionHistoryData(null, startTime, endTime);

                callHistoryList = callHistoryList.Concat(bvSpCallHistoryDataEntities).ToList();
            }

            return callHistoryList;
        }

        private IEnumerable<CallHistoryDataEntity> GetCallHistoryEntities(IEnumerable<BvSpGetInterviewerBreaksEntity> entities)
        {
            return entities.Select(
                x => new CallHistoryDataEntity
                {
                    FiredTime = x.StartTime,
                    ProjectID = GetBreakType(x),
                    Name = x.SurveyName,
                    InterviewID = null,
                    InterviewerID = x.InterviewerId,
                    InterviewerName = x.InterviewerName,
                    TelephoneNumber = "",
                    ExtendedStatus = null,
                    Duration = x.Duration,
                    WaitingTime = null,
                    CallCenterName = x.CallCenterName
                });
        }

        private static string GetBreakType(BvSpGetInterviewerBreaksEntity x)
        {

            var breakTypeDescription = x.IsPaid.HasValue
                ? $"{x.BreakTypeName} {(x.IsPaid == true ? "Paid" : "Unpaid")}"
                : "DELETED BREAKTYPE";

            return $"BREAK {breakTypeDescription} {x.ProjectId ?? ""}";
        }

        private IEnumerable<CallHistoryDataEntity> GetCallHistoryEntities(IEnumerable<PersonSessionHistoryEntity> entities)
        {
            var result = new List<CallHistoryDataEntity>();

            foreach (var historyEntity in entities)
            {
                int? duration = null;
                if (historyEntity.LogoutTime.HasValue)
                {
                    duration = (int)(historyEntity.LogoutTime - historyEntity.LoginTime).Value.TotalSeconds;
                }

                result.Add(new CallHistoryDataEntity
                {
                    FiredTime = historyEntity.LoginTime,
                    ProjectID = "LOGIN",
                    Name = "",
                    InterviewID = null,
                    InterviewerID = historyEntity.InterviewerId,
                    InterviewerName = historyEntity.InterviewerName,
                    TelephoneNumber = "",
                    ExtendedStatus = null,
                    Duration = duration,
                    WaitingTime = null,
                    CallCenterName = historyEntity.CallCenterName
                });

                if (historyEntity.LogoutTime.HasValue)
                {
                    result.Add(new CallHistoryDataEntity
                    {
                        FiredTime = historyEntity.LogoutTime,
                        ProjectID = "LOGOUT",
                        Name = "",
                        InterviewID = null,
                        InterviewerID = historyEntity.InterviewerId,
                        InterviewerName = historyEntity.InterviewerName,
                        TelephoneNumber = "",
                        ExtendedStatus = null,
                        Duration = null,
                        WaitingTime = null,
                        CallCenterName = historyEntity.CallCenterName
                    });
                }
            }

            return result;
        }
    }
}
