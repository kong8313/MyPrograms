using System;
using System.Linq;
using System.Data.SqlClient;
using BvDotNetEngine;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Schedules2007.BvDotNetEngine;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.SampleServiceImplementation;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.DAL.Framework;
using System.Data;

namespace Confirmit.CATI.Core.Repositories
{
    public class InterviewRepository : IInterviewRepository
    {
        private readonly Lazy<ScheduleScriptExecutor> _scheduleScriptExecutor =
            new Lazy<ScheduleScriptExecutor>(() => new ScheduleScriptExecutor());

        BvInterviewWithOriginEntity IInterviewRepository.GetById(int surveySid, int interviewId)
        {
            return GetById(surveySid, interviewId);
        }

        BvInterviewWithOriginEntity IInterviewRepository.GetByIdWithCheck(int surveySid, int interviewId)
        {
            return GetByIdWithCheck(surveySid, interviewId);
        }

        public BvInterviewWithOriginEntity GetByTelephoneNumber(int surveyId, string telephoneNumber)
        {
            var entities = BvInterviewAdapter.GetByCondition("[SurveySID] = @SurveySID AND [TelephoneNumber] = @TelephoneNumber",
                new SqlParameter("@SurveySID", surveyId),
                new SqlParameter("@TelephoneNumber", telephoneNumber)).Select(x => new BvInterviewWithOriginEntity(x));

            return entities.OrderBy(x => x.ID).FirstOrDefault();
        }

        void IInterviewRepository.Update(BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions schedulingOptions)
        {
            Update(interview, schedulingOptions);
        }

        [CanBeNull]
        public static BvInterviewWithOriginEntity GetById(int surveySid, int interviewId)
        {
            var entities = BvInterviewAdapter.GetByCondition("[SurveySID] = @SurveySID AND [ID] = @InterviewID",
                    new SqlParameter("@SurveySID", surveySid),
                    new SqlParameter("@InterviewID", interviewId)).Select(x => new BvInterviewWithOriginEntity(x));

            return entities.FirstOrDefault();
        }

        [NotNull]
        public static BvInterviewWithOriginEntity GetByIdWithCheck(int surveySid, int interviewId)
        {
            var result = GetById(surveySid, interviewId);

            if (result == null)
                throw new InternalErrorException(String.Format(
                    "Interview {0} for survey {1} not found.", interviewId, surveySid));

            return result;
        }

        public void InsertOnly(BvInterviewEntity interview)
        {
            var options = new SchedulingScriptExecutionOptions() { IsExecuteSchedulingScript = false, IsLogToHistory = false };
            Insert(new BvInterviewWithOriginEntity(interview), options);
        }

        public void Insert(
            [NotNull] BvInterviewWithOriginEntity interview,
            SchedulingScriptExecutionOptions schedulingOptions, ISampleDataStorage sampleStorage = null)
        {
            ParameterValidator.ValidateNotNull<BvInterviewEntity>(interview, "interview");
            ParameterValidator.ValidateNotEqual(0, interview.ID, "interview.ID");

            var evt = new InsertInterviewEvent();

            _scheduleScriptExecutor.Value.ScheduleInterview(interview, schedulingOptions);
            evt.AddTiming("ExecuteSchedulingScript");

            //
            //TODO: We needn't it in future
            //
            if (interview.LastCallTime == null)
            {
                interview.LastCallTime = new DateTime(1899, 12, 30, 0, 0, 0);
            }
            if (interview.TimezoneID == 0)
            {
                interview.TimezoneID = null;
            }

            if (sampleStorage != null)
            {
                sampleStorage.InsertInterview(interview);
            }
            else if (schedulingOptions.BatchID != 0)
            {
                ServiceLocator.Resolve<ISampleDataStorageRepository>().Get(schedulingOptions.BatchID).InsertInterview(interview);
            }
            else
            {
                var survey = SurveyRepository.GetById(interview.SurveySID);
                evt.SurveySid = interview.SurveySID;
                evt.SurveyName = survey.Name;
                evt.InterviewId = interview.ID;
                evt.PhoneNumber = interview.TelephoneNumber;

                BvSpInterview_InsertAdapter.ExecuteNonQuery(
                    interview.ID,
                    interview.SurveySID,
                    interview.TimezoneID,
                    interview.TransientState,
                    interview.LastCallPersonSID,
                    interview.Duration,
                    interview.TelephoneNumber,
                    interview.RespondentName,
                    interview.LastCallTime,
                    interview.ExtensionNumber,
                    interview.LastChannelID,
                    interview.ConfirmitSid,
                    interview.DialingMode,
                    interview.IsSentToReview,
                    interview.DialTypeId);
                evt.AddTiming("BvSpInterview_InsertAdapter");

                evt.Save();
            }
        }

        public static void Update([NotNull] BvInterviewWithOriginEntity interview, SchedulingScriptExecutionOptions schedulingOptions)
        {
            ParameterValidator.ValidateNotNull<BvInterviewEntity>(interview, "interview");
            ParameterValidator.ValidateNotEqual(0, interview.ID, "interview.ID");

            var evt = new UpdateInterviewEvent();

            new ScheduleScriptExecutor().ScheduleInterview(interview, schedulingOptions);

            evt.AddTiming("ExecuteSchedulingScript");

            if (schedulingOptions.ExecutionReason == SchedulingScriptExecutionReason.AddedBySample)
            {
                ServiceLocator.Resolve<ISampleDataStorageRepository>().Get(schedulingOptions.BatchID).UpdateInterview(interview);
            }
            else
            {
                var survey = SurveyRepository.GetById(interview.SurveySID);
                evt.SurveySid = interview.SurveySID;
                evt.SurveyName = survey.Name;
                evt.InterviewId = interview.ID;
                evt.PhoneNumber = interview.TelephoneNumber;
                BvSpInterview_UpdateAdapter.ExecuteNonQuery(
                    interview.ID,
                    interview.SurveySID,
                    interview.TimezoneID,
                    interview.TransientState,
                    interview.LastCallPersonSID,
                    interview.Duration,
                    interview.TelephoneNumber,
                    interview.RespondentName,
                    interview.LastCallTime,
                    interview.ExtensionNumber,
                    interview.LastChannelID,
                    interview.DialingMode,
                    interview.DialerId,
                    interview.IsSentToReview);

                evt.AddTiming("BvSpInterview_Update");

                ServiceLocator.Resolve<ISurveyDatabaseService>()
                    .UpdateIts(interview.SurveySID, interview.ID, interview.TransientState);
                evt.AddTiming("SurveyDatabaseService.UpdateIts");
                evt.Save();
            }
        }

        public static void UpdateOnly([NotNull] BvInterviewEntity interview)
        {
            ParameterValidator.ValidateNotNull(interview, "interview");
            ParameterValidator.ValidateNotEqual(0, interview.ID, "interview.ID");

            var evt = new UpdateInterviewEvent();

            BvSpInterview_UpdateAdapter.ExecuteNonQuery(
                interview.ID,
                interview.SurveySID,
                interview.TimezoneID,
                interview.TransientState,
                interview.LastCallPersonSID,
                interview.Duration,
                interview.TelephoneNumber,
                interview.RespondentName,
                interview.LastCallTime,
                interview.ExtensionNumber,
                interview.LastChannelID,
                interview.DialingMode,
                interview.DialerId,
                interview.IsSentToReview);
            evt.AddTiming("BvSpInterview_Update");

            evt.Save();
        }

        public static int[] GetInterviewIdsWithoutRespondents(int surveyId)
        {
            var repricationTableName = ServiceLocator.Resolve<IReplicationSchemaInfoService>().GetDestinationTableName(surveyId);

            var query = $@"SELECT ID 
                           FROM BvInterview i 
                           WHERE SurveySID = @SurveySID AND
                           NOT EXISTS ( SELECT 1 FROM {repricationTableName} r WHERE i.ID = r.respid)";

            var result = new DatabaseEngine().ExecuteScalarList<int>(query, CommandType.Text, new SqlParameter("@SurveySID", surveyId));
            return result.ToArray();
        }
    }
}
