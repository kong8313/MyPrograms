using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Validators;
using Confirmit.CATI.Core.Validators.Interfaces;
using ConfirmitDialerInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.ScheduleDom.Scheduling;

namespace Confirmit.CATI.Core.Services.SampleServiceImplementation
{
    public class SimpleSchedulingSampleRecordProcessor : ISampleRecordProcessor
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IInterviewRepository _interviewRepository;
        private readonly IMultipleAssignmentValidator _multipleAssigmentValidator;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IScheduleService _scheduleService;
        
        private IFCDSettings FcdSettings { get; set; }
        private SampleContext Context { get; set; }
        private int[] PersonGroupsIds { get; set; }
        private HashSet<int> AvailableItses { get; set; }
        private int[] PersonsIds { get; set; }
        private readonly Dictionary<string, int> _assignmentResourceCache;
        private List<BvSpShiftType_ListEntity> _shiftTypes;

        public SimpleSchedulingSampleRecordProcessor(SampleContext context, IFCDSettings fcdSettings)
        {
            FcdSettings = fcdSettings;
            Context = context;

            int catiRootGroupId = PersonGroupService.RootGroupId;
            PersonGroupsIds = PersonGroupService.GetAllChildNotAdministrativeGroupSids(PersonGroupService.RootGroupId)
                .Union(new List<int> { catiRootGroupId }).ToArray();
            PersonsIds = PersonRepository.GetAll().Select(x => x.SID).ToArray();
            var defaultStateGroupId = StateGroupRepository.GetDefault().ID;
            AvailableItses = new HashSet<int>(StateRepository.GetAll(defaultStateGroupId).Select(x => x.StateID));

            _assignmentService = ServiceLocator.Resolve<IAssignmentService>();
            _interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
            _scheduleService = ServiceLocator.Resolve<IScheduleService>();
            
            _multipleAssigmentValidator = ServiceLocator.Resolve<IMultipleAssignmentValidator>();
            _assignmentResourceCache = new Dictionary<string, int>();

            var scheduleId = _surveyRepository.GetById(context.Survey.SID).ScheduleID;
            _shiftTypes = _scheduleService.GetShiftTypeList(scheduleId);
        }

        public void Process(ISampleRecordStorage storage, SampleProcessingStateContainer stateContainer, RespondentRecord record, BvInterviewWithOriginEntity interview, ProcessSampleMode processSampleMode)
        {
            SampleRecordOperationType result = TrySetExtendedStatus(record, interview);
            stateContainer.AddSetExtendedStatusResult(result);

            if (record.IsTelephoneInBlackList)
            {
                interview.TransientState = (int)CallOutcome.Blacklist;
            }
            else if (IsInterviewFilteredByFcd(record, interview) && FcdSettings.AlgorithmType == FcdAlgorithmType.DeleteCalls)
            {
                interview.TransientState = (int)CallOutcome.FilteredByCallDelivery;
            }

            var options = new SchedulingScriptExecutionOptions
            {
                ExecutionReason = SchedulingScriptExecutionReason.AddedBySample,
                IsExecuteSchedulingScript = false,
                BatchID = Context.BatchId,
                ProcessSampleMode = processSampleMode,
                SchedulingScriptNotificatorExceptions = Context.SchedulingScriptNotificatorExceptions,
                IsLogToHistory = false,
                opType = OperationType.SimpleAddSchedulingSample
            };

            _interviewRepository.Insert(interview, options);

            if ((CallOutcome)interview.TransientState == CallOutcome.FreshSample)
            {
                var call = new BvCallEntity
                {
                    SurveySID = Context.Survey.SID,
                    InterviewID = interview.ID,
                    TimeZoneID = interview.TimezoneID.GetValueOrDefault(),
                    ShiftID = (int)CallShiftType.AnyValid,
                    Priority = 1,
                    Status = 0,
                    CellId = record.ClusteredCellId,
                    DialTypeId = interview.DialTypeId
                };

                result = TrySetTimeToCall(record, interview, call);
                stateContainer.AddSetTimeToCallResult(result);

                result = TrySetTimeToExpire(record, interview, call);
                stateContainer.AddSetTimeToExpireResult(result);

                result = TrySetCallPriority(record, call);
                stateContainer.AddSetCallPriorityResult(result);

                result = TrySetShiftType(record, call);
                stateContainer.AddSetShiftTypeResult(result);
                
                result = TrySetCallState(record, call);
                stateContainer.AddSetCallStateResult(result);
                
                bool multipleAssignment;
                result = TryAssignResources(record, call, PersonGroupsIds, PersonsIds, out multipleAssignment);

                if (!multipleAssignment)
                {
                    stateContainer.AddAssignResourceResult(result);
                }
                else
                {
                    stateContainer.AddAssingResourcesResult(result);
                }

                CallQueueService.AddCall(call, Context.BatchId, interview, SchedulingScriptExecutionReason.AddedBySample);
            }
        }

        public void OnCompleted()
        {
            _assignmentResourceCache.Clear();
        }

        private bool IsInterviewFilteredByFcd(RespondentRecord record, BvInterviewEntity interview)
        {
            return record.IsClosedCell && !Context.IgnoredItsByFcd.Contains(interview.TransientState);
        }

        /// <summary>
        /// Here we are trying to set extended status for the interview created for the current sample record.
        /// </summary>
        /// <param name="record">Current sample record.</param>
        /// <param name="interview">Interview created for current sample record.</param>
        /// <remarks>
        /// We get extended status from CatiExtendedStatus column of a respondent table of survey DB.
        /// This column does not usually exists, so a background variable "CatiExtendedStatus" 
        /// should be added to the survey and filled in from the sample to make this work.
        /// </remarks>
        private SampleRecordOperationType TrySetExtendedStatus(RespondentRecord record, BvInterviewEntity interview)
        {
            if (record.CatiExtendedStatus == null || record.CatiExtendedStatus.Trim() == string.Empty)
            {
                return SampleRecordOperationType.Empty;
            }

            int itsFromSample;
            Int32.TryParse(record.CatiExtendedStatus, out itsFromSample);

            // If status is not fresh sample - we suppose that it may be filtered by call delivery.
            if (AvailableItses.Contains(itsFromSample) &&
                (CallOutcome)interview.TransientState == CallOutcome.FreshSample)
            {
                interview.TransientState = itsFromSample;

                return SampleRecordOperationType.Correct;
            }

            return SampleRecordOperationType.Incorrect;
        }

        private SampleRecordOperationType TryAssignResources(
            RespondentRecord record,
            BvCallEntity call,
            IEnumerable<int> personGroupsSids,
            IEnumerable<int> catiPersonsIds,
            out bool multipleAssignment)
        {
            multipleAssignment = false;

            if (string.IsNullOrWhiteSpace(record.ResourceIds))
            {
                return TryAssignResource(record.Resource, call, personGroupsSids, catiPersonsIds);
            }

            multipleAssignment = true;

            int assignmentResourceId;

            if (_assignmentResourceCache.ContainsKey(record.ResourceIds))
            {
                assignmentResourceId = _assignmentResourceCache[record.ResourceIds];
            }
            else
            {
                int[] resourceIds;
                try
                {
                    resourceIds = GetResourceIdsByResourceIdsString(record.ResourceIds);
                }
                catch
                {
                    return SampleRecordOperationType.Incorrect;
                }

                SampleRecordOperationType validationResult = ValidateMultipleAssignment(resourceIds);
                if (validationResult != SampleRecordOperationType.Correct)
                {
                    return validationResult;
                }

                assignmentResourceId = _assignmentService.GetAssignmentResourceId(resourceIds.Distinct().ToArray());
                _assignmentResourceCache.Add(record.ResourceIds, assignmentResourceId);
            }

            call.Resource = assignmentResourceId;

            return SampleRecordOperationType.Correct;
        }

        private int[] GetResourceIdsByResourceIdsString(string resourceIdsString)
        {
            return resourceIdsString
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();
        }

        private SampleRecordOperationType ValidateMultipleAssignment(IEnumerable<int> resourceIds)
        {
            var groupsCount = 0;
            var personsCount = 0;

            foreach (int resourceId in resourceIds)
            {
                if (PersonGroupsIds.Contains(resourceId))
                {
                    groupsCount++;
                }
                else if (PersonsIds.Contains(resourceId))
                {
                    personsCount++;
                }
                else
                {
                    return SampleRecordOperationType.Incorrect;
                }
            }

            if (_multipleAssigmentValidator.ValidateMultipleAssignmentByCounts(groupsCount, personsCount) !=
                MultipleAssignmentValidationResult.Success)
            {
                return SampleRecordOperationType.Incorrect;
            }

            return SampleRecordOperationType.Correct;
        }

        /// <summary>
        /// Here we are trying to assign a resource (interviewer or group) to the call created for the current sample record.
        /// This should be done for simple scheduling mode only.
        /// </summary>
        /// <param name="resourceId">Current resource id.</param>
        /// <param name="call">Call created for current sample record.</param>
        /// <param name="personGroupsSids">List of IDs of all person groups.</param>
        /// <param name="catiPersonsIds">List of cati persons</param>
        /// <returns></returns>
        /// <remarks>
        /// We get resource ID from CatiInterviewerId column of a respondent table of survey DB.
        /// This column always exists for CATI surveys and could be filled in from sample file.
        /// </remarks>
        private static SampleRecordOperationType TryAssignResource(int resourceId, BvCallEntity call, IEnumerable<int> personGroupsSids, IEnumerable<int> catiPersonsIds)
        {
            if (resourceId == 0)
            {
                return SampleRecordOperationType.Empty;
            }

            if (catiPersonsIds.Contains(resourceId) ||
                personGroupsSids.Contains(resourceId))
            {
                call.Resource = resourceId;
                return SampleRecordOperationType.Correct;
            }

            return SampleRecordOperationType.Incorrect;
        }

        /// <summary>
        /// Here we are trying to set time to call for the call created for the current sample record.
        /// This should be done for simple scheduling mode only.
        /// </summary>
        /// <param name="record">Current sample record.</param>
        /// <param name="interview">Interview created for current sample record.</param>
        /// <param name="call">Call created for current sample record.</param>
        /// <remarks>
        /// We get time to call from CatiCallTime column of a respondent table of survey DB.
        /// This column does not usually exists, so a background variable "CatiCallTime" 
        /// should be added to the survey and filled in from the sample to make this work.
        /// </remarks>
        private SampleRecordOperationType TrySetTimeToCall(RespondentRecord record, BvInterviewEntity interview, BvCallEntity call)
        {
            if (String.IsNullOrWhiteSpace(record.CatiCallTime))
            {
                return SampleRecordOperationType.Empty;
            }

            var timezoneService = ServiceLocator.Resolve<ITimezoneService>();
            DateTime timeToCall;

            if (DateTime.TryParse(record.CatiCallTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out timeToCall))
            {
                int respondentTimezoneId = timezoneService.GetTimezoneIdOrDefaultCallCenterTimezoneId(interview.TimezoneID);
                call.TimeInShift = TimezoneService.ConvertTimeToUtc(respondentTimezoneId, timeToCall);

                return SampleRecordOperationType.Correct;
            }

            return SampleRecordOperationType.Incorrect;
        }

        private SampleRecordOperationType TrySetCallPriority(RespondentRecord record, BvCallEntity call)
        {
            if (String.IsNullOrWhiteSpace(record.CatiCallPriority))
            {
                return SampleRecordOperationType.Empty;
            }

            int priority;

            if (int.TryParse(record.CatiCallPriority, out priority))
            {
                if (priority < 1)
                    return SampleRecordOperationType.Incorrect;

                call.Priority = priority;
                return SampleRecordOperationType.Correct;
            }

            return SampleRecordOperationType.Incorrect;
        }

        private SampleRecordOperationType TrySetShiftType(RespondentRecord record, BvCallEntity call)
        {
            if (String.IsNullOrWhiteSpace(record.CatiShiftType))
            {
                return SampleRecordOperationType.Empty;
            }

            BvSpShiftType_ListEntity shiftType;

            if (int.TryParse(record.CatiShiftType, out var shiftTypeId))
            {
                if (shiftTypeId == 0 || shiftTypeId == -1)
                {
                    call.ShiftID = shiftTypeId == -1 ? (int)CallShiftType.AnyValid : (int)CallShiftType.None;
                    return SampleRecordOperationType.Correct;
                }

                shiftType = _shiftTypes.SingleOrDefault(x => x.ID == shiftTypeId);
            }
            else
            {
                shiftType = _shiftTypes.SingleOrDefault(x => x.Name == record.CatiShiftType);
            }

            if (shiftType != null)
            {
                call.ShiftID = shiftType.ObjectID.Value;
                return SampleRecordOperationType.Correct;
            }

            return SampleRecordOperationType.Incorrect;
        }

        private SampleRecordOperationType TrySetCallState(RespondentRecord record, BvCallEntity call)
        {
            if (String.IsNullOrWhiteSpace(record.CatiCallState))
            {
                return SampleRecordOperationType.Empty;
            }

            if (int.TryParse(record.CatiCallState, out var state))
            {
                if (state != 0 && state != 1)
                    return SampleRecordOperationType.Incorrect;

                call.CallState = state == 1 ? (int)CallState.Scheduled : (int)CallState.DisabledByUser;
                return SampleRecordOperationType.Correct;
            }

            return SampleRecordOperationType.Incorrect;
        }

        /// <summary>
        /// Here we are trying to set expiration time to call for the call created for the current sample record.
        /// This should be done for simple scheduling mode only.
        /// </summary>
        /// <param name="record">Current sample record.</param>
        /// <param name="interview">Interview created for current sample record.</param>
        /// <param name="call">Call created for current sample record.</param>
        /// <remarks>
        /// We get time to expire from CatiCallExpirationTime column of a respondent table of survey DB.
        /// This column does not usually exists, so a background variable "CatiCallExpirationTime" 
        /// should be added to the survey and filled in from the sample to make this work.
        /// </remarks>
        private SampleRecordOperationType TrySetTimeToExpire(RespondentRecord record, BvInterviewEntity interview, BvCallEntity call)
        {
            if (String.IsNullOrWhiteSpace(record.CatiCallExpirationTime))
            {
                return SampleRecordOperationType.Empty;
            }

            var timezoneService = ServiceLocator.Resolve<ITimezoneService>();

            if (DateTime.TryParse(record.CatiCallExpirationTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expirationTime))
            {
                int respondentTimezoneId = timezoneService.GetTimezoneIdOrDefaultCallCenterTimezoneId(interview.TimezoneID);
                call.TimeToExpire = TimezoneService.ConvertTimeToUtc(respondentTimezoneId, expirationTime);

                return SampleRecordOperationType.Correct;
            }

            return SampleRecordOperationType.Incorrect;
        }
    }
}