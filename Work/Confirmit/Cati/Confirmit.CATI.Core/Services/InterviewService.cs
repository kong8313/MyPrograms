using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Security.Crypto.Web;
using ConfirmitDialerInterface;
using Microsoft.Practices.ObjectBuilder2;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Core.Services
{
    public class InterviewService : IInterviewService
    {
        private const int BatchSize = 3000;
        
        private readonly Lazy<ITimezoneService> _timezoneService;
        private readonly Lazy<IShiftServiceFactory> _shiftServiceFactory;
        private readonly Lazy<IReplicationService> _replicationService;


        public InterviewService()
        {
            // Note, that objects of this class can be created from javascript code
            // That's why the ctor should be without parameters and all dependecies 
            // should be resolved in the ctor body here
            _timezoneService = new Lazy<ITimezoneService>(() => ServiceLocator.Resolve<ITimezoneService>());
            _shiftServiceFactory = new Lazy<IShiftServiceFactory>(() => ServiceLocator.Resolve<IShiftServiceFactory>());
            _replicationService = new Lazy<IReplicationService>(() => ServiceLocator.Resolve<IReplicationService>());
        }

        public static void Schedule(
            int surveySid,
            int interviewId,
            SchedulingScriptExecutionOptions options)
        {
            var interview = InterviewRepository.GetById(surveySid, interviewId);

            InterviewRepository.Update(interview, options);
        }

        internal static int? SafeIncrementAndFetchCallAttemptCount(int surveySid, int interviewId, DialingMode dialingMode)
        {
            try
            {
                if (dialingMode == DialingMode.Automatic || dialingMode == DialingMode.Predictive)
                {
                    return ServiceLocator.Resolve<ISurveyDatabaseService>().IncrementCallAttemptCount(surveySid, interviewId);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error increment call attempt count. Exception details: " + e);
                return null;
            }
            
            return null;
        }

        /// <summary>
        /// Gets the interview respondent timezone ID for the specified survey SID and interview ID
        ///  or local company timezone if it is not specified in the interview.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <param name="interviewId">The interview ID.</param>
        /// <returns>Respondent timezone ID</returns>
        public int GetInterviewTimezoneOrDefault(int surveySid, int interviewId)
        {
            var interview = InterviewRepository.GetById(surveySid, interviewId);

            return GetInterviewTimezoneOrDefault(interview);
        }

        /// <summary>
        /// Gets the interview respondent timezone ID for the specified survey SID and interview ID
        ///  or local company timezone if it is not specified in the interview.
        /// </summary>
        /// <param name="interview">The interview.</param>
        /// <returns>Respondent timezone ID</returns>
        public int GetInterviewTimezoneOrDefault(BvInterviewEntity interview)
        {
            return _timezoneService.Value.GetTimezoneIdOrDefaultCallCenterTimezoneId(interview.TimezoneID);
        }

        public void AddAppointments(
            int surveySid,
            int interviewId,
            int batchId,
            Appointment[] appointments,
            bool allowOutsideShift)
        {
            int timezoneId = GetInterviewTimezoneOrDefault(surveySid, interviewId);

            //
            // should be implemented later
            if (batchId > 0)
            {
                // TODO:
            }

            //
            // check new appointments
            var survey = SurveyRepository.GetById(surveySid);

            var shiftService = _shiftServiceFactory.Value.Get(survey.ScheduleID);
            if (!allowOutsideShift)
            {
                foreach (var appointment in appointments)
                {
                    // check whether appintment time is inside assigned schedule shifts                    
                    var shift = shiftService.GetExactShift(appointment.time, timezoneId); // should be UTC time

                    if (shift == null)
                    {
                        var localAppointmentTime =
                            _timezoneService.Value.ConvertTimeFromUtc(timezoneId, appointment.time);

                        throw new UserMessageException(
                            String.Format("Appointment with date {0} is out of shifts", localAppointmentTime),
                            "Error_AppointmentOutOfShifts");
                    }
                }
            }

            //
            // all following code can be moved to single SP

            //
            // delete unused appointments
            var existingAppointments = BvSpAppointmentGet2Adapter.ExecuteEntityList(
                surveySid,
                interviewId);

            foreach (var existingAppointment in existingAppointments)
            {
                bool foundInListOfNewAppointments = false;

                foreach (var appointment in appointments)
                {
                    if (existingAppointment.ID == appointment.id)
                    {
                        foundInListOfNewAppointments = true;
                        break;
                    }
                }

                if (!foundInListOfNewAppointments)
                {
                    var databaseEngine = new DatabaseEngine();

                    databaseEngine.ExecuteNonQuery(
                        "DELETE FROM [BvAppointment] WHERE [SurveySID] = @SurveySID AND [ID] = @ID",
                        CommandType.Text,
                        new SqlParameter("@SurveySID", surveySid),
                        new SqlParameter("@ID", existingAppointment.ID));
                }
            }

            //
            // create/update appointments
            foreach (var appointment in appointments)
            {
                if (appointment.id == 0)
                {
                    appointment.state = 0;
                }

                var bvAppointmentEntity = new BvAppointmentEntity
                {
                    ID = appointment.id,
                    SurveySID = surveySid,
                    InterviewSID = interviewId,
                    Time = appointment.time,
                    ExpTime = appointment.expirationTime,
                    ContactName = appointment.contactName,
                    State = (int)appointment.state,
                    TZID = appointment.appointmentTimeZone != null ? appointment.appointmentTimeZone.Id : timezoneId
                };
                AppointmentRepository.InsertUpdate(bvAppointmentEntity);
            }
        }

        public static string GetPhoneNumber(int surveyId, int interviewId)
        {
            var interview = InterviewRepository.GetByIdWithCheck(surveyId, interviewId);
            return interview.TelephoneNumber;
        }

        public void DeleteRespondents(int surveySid, int[] respondentIDs, CancellationToken cancellationToken)
        {
            // Delete all associating tasks from BvTasks table
            var batches = respondentIDs.ToList().SplitIntoBatches(BatchSize);
            foreach (var respondentIdsBatch in batches)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var batch = TransferBatch.Create())
                {
                    var batchId = batch.Value;

                    batch.Insert(respondentIdsBatch);

                    Trace.TraceInformation("Deleting respondents for batch ID {0}", batchId);

                    DeleteAssociatedTasks(surveySid, batchId);

                    DeleteCallsAndBatch(surveySid, batchId);
                }
            }
        }

        private static void DeleteCallsAndBatch(int surveySid, int batchId)
        {
            using (var dbTransactionScope = new DatabaseTransactionScope("DeleteRespondents"))
            {
                CallQueueService.DeleteCalls(surveySid, batchId);

                BvSpInterviewsAndAppointments_Delete_BatchAdapter.ExecuteNonQuery(surveySid, batchId);

                dbTransactionScope.Commit();
            }
        }

        private static void DeleteAssociatedTasks(int surveySid, int batchId)
        {
            var tasks = BvTasksAdapter.GetByCondition(
                "SurveySID = @SurveySID AND interviewID IN ( SELECT ItemID FROM BvTransferArrays WHERE BatchID = @BatchID )",
                new SqlParameter("@SurveySID", surveySid),
                new SqlParameter("@BatchID", batchId));

            foreach (var task in tasks)
            {
                TaskService.TerminateTask(task, new DatabaseTransactionOptions("MgtSrv.DeleteRespondents", DeadlockPriority.Normal));
            }
        }

        public static int GetCountOfInterviewsWithSpecificITSs(int surveyId, int[] itsIDs)
        {
            if (itsIDs.Length <= 0)
            {
                return 0;
            }

            return BvSampleStatusSummaryAdapter.GetByCondition(
                    String.Format("SurveySID = {0} AND ITS IN ({1})",
                    surveyId,
                    String.Join(",", itsIDs.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray()))).Sum(x => x.Cnt);
        }

        public BvInterviewWithOriginEntity AddRespondent(BvSurveyEntity survey, int respondentId, int its,
            OperationType operationType, Role role, int? personSid = null)
        {
            return AddRespondent(survey, respondentId, new SchedulingScriptExecutionOptions
            {
                ITS = its,
                IsLogToHistory = role == Role.WebRespondent,
                ExecutionReason = SchedulingScriptExecutionReason.Added,
                RoleID = (int)role,
                opType = operationType,
                LastCallPersonSID = personSid
            });
        }

        public BvInterviewWithOriginEntity AddRespondent(BvSurveyEntity survey, int respondentId, SchedulingScriptExecutionOptions options)
        {
            using (var dbLock = DatabaseLockService.CreateLock(
                DatabaseLockTimeoutsAndRecourceNames.GetAddRespondentToCatiResourceName(survey.SID, respondentId),
                "InterviewService.AddRespondent",
                ServiceLocator.Resolve<IReplicationSettings>().ForceReplicationLockTimeout))
            {
                dbLock.EnterLock();

                var respondentDataObtainer = ServiceLocator.Resolve<IRespondentObtainer>();
                var respondentData = respondentDataObtainer.GetRespondent(survey, respondentId);

                if (respondentData.TimeZoneId > 0)
                    TimezoneService.Activate(respondentData.TimeZoneId);

                var interview = InterviewRepository.GetById(survey.SID, respondentId);
                var cfInterview = GetInterviewFromRespondentRecord(survey.SID, 0, respondentData);

                if (interview == null)
                {
                    var interviewRepository = ServiceLocator.Resolve<IInterviewRepository>();
                    interviewRepository.Insert(cfInterview, options);
                }
                else
                {
                    if (options.RoleID != (int)Role.WebRespondent)
                    {
                        throw new UserMessageException($"Cannot update interview for role '{options.RoleID}' because operation is not defined");
                    }

                    cfInterview.BatchID = interview.BatchID;
                    cfInterview.DialingMode = interview.DialingMode;
                    cfInterview.LastCallPersonSID = interview.LastCallPersonSID;
                    cfInterview.TransientState = options.ITS;

                    InterviewRepository.Update(cfInterview, new SchedulingScriptExecutionOptions { RoleID = (int)Role.WebRespondent, opType = OperationType.UpdateRecordInWebInterview });
                }

                _replicationService.Value.ReplicateInterviewData(survey, respondentId);

                return cfInterview;
            }
        }

        public static void SetDialingModeForBatch(int surveyId, int dialingMode, int batchId)
        {
            new DatabaseEngine().ExecuteNonQuery(
                "UPDATE [BvInterview] SET [DialingMode] = @DialingMode FROM [BvTransferArrays ] ta WHERE " +
                "BvInterview.ID = ta.ItemID AND ta.BatchID = @BatchID AND BvInterview.SurveySID = @SurveyId",
                CommandType.Text,
                new SqlParameter("@DialingMode", dialingMode),
                new SqlParameter("@SurveyId", surveyId),
                new SqlParameter("@BatchID", batchId));
        }

        internal static BvInterviewWithOriginEntity GetInterviewFromRespondentRecord(int surveyId, int batchId, RespondentRecord record, SampleContext context = null)
        {
            var transientState = context != null && context.ProcessSampleMode == ProcessSampleMode.Update ?
                record.TransientState : (int)CallOutcome.FreshSample;

            return new BvInterviewWithOriginEntity(
                new BvInterviewEntity()
                {
                    ID = record.InterviewId,
                    SurveySID = surveyId,
                    RespondentName = record.RespondentName,
                    TelephoneNumber = record.RespondentPhone,
                    LastCallTime = record.LastCallTime,
                    Duration = record.TotalDuration,
                    ExtensionNumber = record.ExtensionNumber,
                    TimezoneID = record.TimeZoneId,
                    ConfirmitSid = record.Sid,
                    BatchID = batchId,
                    LastChannelID = record.LastChannelId,
                    TransientState = transientState,
                    DialingMode = (byte)record.DialMode,
                    DialTypeId = record.DialTypeId
                });
        }

        public void BindDialerIdToInterview(int surveyId, int interviewId, int dialerId)
        {
            var interview = InterviewRepository.GetByIdWithCheck(surveyId, interviewId);

            BindDialerIdToInterview(interview, dialerId);
        }

        public void BindDialerIdToInterview(BvInterviewEntity interview, int dialerId)
        {
            interview.DialerId = dialerId;
            InterviewRepository.UpdateOnly(interview);
        }

        public string GenereteSecurityKey(BvInterviewEntity interview)
        {
            var securityKey = interview.ConfirmitSid.TrimEnd();

            var sid = EncryptionUsingMachineKey.Encrypt(
                DataProtection.All,
                string.Format("r&{0}&s&{1}&__channel&cati", interview.ID, securityKey));

            return sid;
        }

        public int[] GetInterviewIdsWithoutRespondents(int surveyId)
        {
            EventDetailsScope.Current.AddTiming("Count respondents difference");
            using (var connectionScope = new ConnectionScope())
            {
                var result = InterviewRepository.GetInterviewIdsWithoutRespondents(surveyId);

                EventDetailsScope.Current.AddTiming($"Respondents difference - {result?.Length ?? 0}");
                return result;
            }
        }
    }
}
