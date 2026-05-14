using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BvCallHandlerLibrary;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;
using Confirmit.CATI.Core.Services.TimeService;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services
{
    internal class CallDeliveryService : ICallDeliveryService
    {
        private readonly Lazy<IPersonGroupRepository> _personGroupRepository;
        private readonly Lazy<ICallRequestFactory> _callRequestFactory;
        private readonly Lazy<IQuotaClusterService> _quotaClusterService;
        private readonly Lazy<ITimeService> _timeService;
        private readonly Lazy<IRespondentVariablesService> _respondentVariablesService;

        public static readonly DateTime NullTime = new DateTime(1899, 12, 30, 0, 0, 0);

        public CallDeliveryService()
        {
            _personGroupRepository = new Lazy<IPersonGroupRepository>(() => ServiceLocator.Resolve<IPersonGroupRepository>());
            _callRequestFactory = new Lazy<ICallRequestFactory>(() => ServiceLocator.Resolve<ICallRequestFactory>());
            _quotaClusterService = new Lazy<IQuotaClusterService>(() => ServiceLocator.Resolve<IQuotaClusterService>());
            _timeService = new Lazy<ITimeService>(() => ServiceLocator.Resolve<ITimeService>());
            _respondentVariablesService = new Lazy<IRespondentVariablesService>(() => ServiceLocator.Resolve<IRespondentVariablesService>());
        }

        //
        // TODO: probably it would be good to move this method somewhere else and to
        // remove this class at all.
        //
        public List<CallInfo> LookupCalls(
            int surveySid,
            int dialerId,
            int? groupId,
            CallsSelectionAlgorithm callsSelectionAlgorithm,
            int count,
            bool isRecording,
            out List<GroupInfo> aggregatedGroupsInfo)
        {
            var callList = new List<CallInfo>();

            IDataReader dataReader;
            var currentTime = ServiceLocator.Resolve<ITimeService>().GetUtcNow();
            var dialeType = ServiceLocator.Resolve<IDialersRepository>().GetById(dialerId)?.DialTypeId ?? 0;
            switch (callsSelectionAlgorithm)
            {
                case CallsSelectionAlgorithm.ByPersonGroup:
                    dataReader = BvSpGetCachedCallsForPredictiveSurveyByPersonGroupAdapter.ExecuteReader(surveySid, groupId, count, currentTime, dialeType);
                    break;
                case CallsSelectionAlgorithm.CallsAssignedToCampaignOnly:
                    dataReader = BvSpGetCachedCallsForPredictiveSurveyAssignedToSurveyOnlyAdapter.ExecuteReader(surveySid, count, currentTime, dialeType);
                    break;
                case CallsSelectionAlgorithm.CallsAssignedToAgentsExplicitly:
                    dataReader = BvSpGetCachedCallsForPredictiveSurveyExplicitlyAssignedAdapter.ExecuteReader(surveySid, dialerId, count, currentTime, dialeType);
                    break;
                case CallsSelectionAlgorithm.ByCampaign:
                default:
                    dataReader = BvSpGetCachedCallsForPredictiveSurveyBySurveyAdapter.ExecuteReader(surveySid, dialerId, count, currentTime, dialeType);
                    break;
            }

            var aggregatedGroupsInfoData = new Dictionary<int, int>();

            using (dataReader)
            {
                var explicitSidOrdinal = dataReader.GetOrdinal("ExplicitSid");
                var timeInShiftOrdinal = dataReader.GetOrdinal("TimeInShift");
                var interviewIdOrdinal = dataReader.GetOrdinal("InterviewID");
                var iDOrdinal = dataReader.GetOrdinal("ID");
                var groupIdOrdinal = dataReader.GetOrdinal("GroupID");
                var telephoneNumberOrdinal = dataReader.GetOrdinal("TelephoneNumber");
                var diallingModeOrdinal = dataReader.GetOrdinal("DiallingMode");
                var expireTimeOrdinal = dataReader.GetOrdinal("ExpireTime");
                // ExtensionNumber field is being used for storing Caller ID
                var callerIdOrdinal = dataReader.GetOrdinal("ExtensionNumber");

                while (dataReader.Read())
                {
                    var agentId = dataReader.GetInt32(explicitSidOrdinal);
                    var callGroupId = dataReader.GetInt32(groupIdOrdinal);

                    if (!aggregatedGroupsInfoData.ContainsKey(callGroupId))
                    {
                        aggregatedGroupsInfoData.Add(callGroupId, 0);
                    }

                    aggregatedGroupsInfoData[callGroupId] = aggregatedGroupsInfoData[callGroupId] + 1;

                    var timeToCall = (DateTime?)dataReader.GetDateTime(timeInShiftOrdinal);
                    if (timeToCall.Equals(NullTime))
                    {
                        timeToCall = null;
                    }

                    var agingTimeout = GetAgingTimeout(dataReader.GetDateTime(expireTimeOrdinal));
                    var callerId = dataReader.IsDBNull(callerIdOrdinal) ? string.Empty : dataReader.GetString(callerIdOrdinal);
                    var phoneNumber = dataReader.IsDBNull(telephoneNumberOrdinal) ? string.Empty : dataReader.GetString(telephoneNumberOrdinal);

                    callList.Add(new CallInfo(
                        agentId,
                        (dataReader.GetInt32(interviewIdOrdinal)),
                        dataReader.GetInt32(iDOrdinal), //TODO CODI changes: propagate callId 'long' type to the CATI DB
                        callGroupId,
                        phoneNumber,
                        timeToCall,
                        dataReader.GetByte(diallingModeOrdinal) == 0 ? DialingMode.Predictive : (DialingMode)Convert.ToInt32(dataReader.GetByte(diallingModeOrdinal)),
                        false, //'wasAbandoned' must be taken from call in fact
                        0, //'attemptsMade' must be taken from call in fact
                        0, // 'previousConnects' must be taken from call in fact
                        0, // 'numberOfNoAnswer' must be taken from call in fact'
                        "", /*'PROTSInternalFlag' where is it kept? */
                        isRecording,
                        agingTimeout,
                        callerId,
                        null));
                }
            }

            aggregatedGroupsInfo = (from aggregatedCounter in aggregatedGroupsInfoData
                select
                    new GroupInfo {
                        GroupId = aggregatedCounter.Key,
                        GroupName = _personGroupRepository.Value.TryGetById(aggregatedCounter.Key) == null ? "<group not found>" : _personGroupRepository.Value.TryGetById(aggregatedCounter.Key).Name,
                        CallsCount = aggregatedCounter.Value
                    }).ToList();

            var respondentVariables = _respondentVariablesService.Value.GetVariablesToSend(surveySid, callList.Select(x => x.interviewId).ToList());

            if (respondentVariables != null)
            {
                foreach (var call in callList)
                {
                    call.respondentVariables = respondentVariables[call.interviewId];
                }
            }

            return callList;
        }

        private int GetAgingTimeout(DateTime expiredTime)
        {
            var now = _timeService.Value.GetUtcNow();
            double timeoutInMin = Math.Round((expiredTime - now).TotalMinutes);

            if (timeoutInMin < DialerEventsHandler.MaxCallAgingTimeoutInMin)
                return Math.Max(1, (int)timeoutInMin);

            return DialerEventsHandler.MaxCallAgingTimeoutInMin;
        }

        public CallRequestResult LookupCall(
            int personId,
            int surveyId,
            int interviewId,
            GetCallEvent activityEvent)
        {
            var request = _callRequestFactory.Value.Create(personId, surveyId, interviewId);

            activityEvent.Details.Description = request.Description;

            return request.Execute();
        }

        public void WrapupCall(BvCallEntity call)
        {
            _quotaClusterService.Value.Decrement(call.SurveySID, call.CellId);
        }
    }
}
