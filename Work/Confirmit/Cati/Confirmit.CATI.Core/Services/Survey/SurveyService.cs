using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using BvCallHandlerLibrary.Tools;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Query;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Reports;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.Services.FilterServiceImplementation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Services.TimeService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services.Survey
{
    public class SurveyService : ISurveyService, ISurveyPublishService, ISurveyCallDistributionService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IReplicationSchemaService _replicationSchemaService;
        private readonly IFcdQuotaService _fcdQuotaService;
        private readonly ITimeService _timeService;
        private readonly ISurveyMetadataCacheService _surveyMetadataCacheService;
        private readonly ITelephony _telephony;
        private readonly IMnTciTools _mnTciTools;
        private readonly IDialerSurveyParametersManager _dialerSurveyParametersManager;
        private readonly ISurveysSettings _systemSettings;

        public SurveyService(ISurveyRepository surveyRepository,
            IScheduleRepository scheduleRepository,
            IReplicationSchemaService replicationSchemaService,
            IFcdQuotaService fcdQuotaService,
            ITimeService timeService,
            ISurveyMetadataCacheService surveyMetadataCacheService,
            ITelephony telephony,
            IMnTciTools mnTciTools,
            IDialerSurveyParametersManager dialerSurveyParametersManager,
            ISurveysSettings systemSettings)
        {
            _surveyRepository = surveyRepository;
            _scheduleRepository = scheduleRepository;
            _replicationSchemaService = replicationSchemaService;
            _fcdQuotaService = fcdQuotaService;
            _timeService = timeService;
            _surveyMetadataCacheService = surveyMetadataCacheService;
            _telephony = telephony;
            _mnTciTools = mnTciTools;
            _dialerSurveyParametersManager = dialerSurveyParametersManager;
            _systemSettings = systemSettings;
        }

        public const int LimitCallsForMoveAndRescheduleAction = 1000;

        public static DialingMode GetDialingMode(int sid)
        {
            return (DialingMode)SurveyRepository.GetById(sid).DialMode;
        }

        DialingMode ISurveyService.GetDialingMode(int sid)
        {
            return GetDialingMode(sid);
        }

        public static void SetDialingMode(int sid, DialingMode dialMode)
        {
            var survey = SurveyRepository.GetById(sid);
            survey.DialMode = (byte)dialMode;
            SurveyRepository.Update(survey);
        }

        public static List<BvSpState_ListEntity> GetTransientStates(int sid)
        {
            using (var rd = BvSpState_ListBySurveyAdapter.ExecuteReader(sid))
            {
                return BvSpState_ListAdapter.ReadList(rd);
            }
        }

        /// <summary>
        /// Returns scheduling parameters list for the specified survey.
        /// </summary>
        /// <param name="surveySid"></param>
        /// <returns></returns>
        public static List<BvScheduleParamEntity> GetSchedulingParametersList(int surveySid)
        {
            return BvScheduleParamAdapter.GetByCondition(
                "[SurveySID] = @SurveySID",
                new SqlParameter("@SurveySID", surveySid));
        }

        /// <summary>
        /// Resets scheduling parameters values for the specified survey.
        /// </summary>
        /// <param name="surveySid"></param>
        /// <returns></returns>
        public static void ResetSchedulingParametersValues(
            int surveySid)
        {
            BvSpScheduleParam_ResetParamAdapter.ExecuteNonQuery(surveySid);
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishScheduleParamsUpdated();
        }

        public static List<BvSpSurvey_GetOpenedEntity> OpenedSurveys
        {
            get
            {
                return BvSpSurvey_GetOpenedAdapter.ExecuteEntityList();
            }
        }

        public static List<BvSpReportSSSEntity> GetSampleStatusSummary(int sid, int? filterId, IEnumerable<int> excludedITSes)
        {
            var filterService = ServiceLocator.Resolve<IFilterService>();
            var sqlFilterProvider = ServiceLocator.Resolve<ISqlFilterProvider>();
            var filter = sqlFilterProvider.TryToGetFilter(filterId, sid);

            if (excludedITSes != null && excludedITSes.Any())
            {
                filter = filterService.ExtendFilter(filter,
                    excludedITSes.Select(x => new SqlCondition("TransientState", TableTypes.Interview,
                        FilterOperator.NotEqual, x.ToString(), VariableTypes.Integer, false)));
            }

            string selectInterviewsQuery = filterService.GenerateSqlWithSelect(
                        filter,
                        sid,
                        FilterGenerateMode.AllInterviewStates);

            return BvSpReportSSSAdapter.ExecuteEntityList(sid, selectInterviewsQuery, BvIntArrayTypeAdapter.CreateTable(excludedITSes ?? new int[] { }));
        }

        public static short GetPriorityFromITS(int sid, int its)
        {
            var surveyEntity = SurveyRepository.GetById(sid);

            var stateEntity = StateRepository.GetByItsAndStateGroupId(its, surveyEntity.StateGroupID);

            return (short)stateEntity.Priority;
        }

        public static BvAppointmentEntity[] GetAppointments(int sid, int interviewId)
        {
            return BvAppointmentAdapter.GetByCondition(
                    @"SurveySID = @SurveySID AND InterviewSID = @InterviewID AND State = 0",
                    new SqlParameter("@SurveySID", sid),
                    new SqlParameter("@InterviewID", interviewId)).ToArray();
        }

        public static BvAppointmentEntity[] GetNotActiveAppointments(int surveySID, int interviewID)
        {
            return BvAppointmentAdapter.GetByCondition(
                    "SurveySID = @SurveySID AND InterviewSID = @InterviewID AND [State] <> 0",
                    new SqlParameter("@SurveySID", surveySID),
                    new SqlParameter("@InterviewID", interviewID))
                    .ToArray();
        }

        public static int GetSampleSize(int sid)
        {
            return new DatabaseEngine().ExecuteScalar<int>(
                "SELECT COUNT( ID ) FROM BvInterview WHERE SurveySID = @SurveySID",
                CommandType.Text,
                new SqlParameter("@SurveySID", sid));
        }

        /// <summary>
        /// Validates project Id format.
        /// Throws exception if format is invalid.
        /// </summary>
        /// <param name="projectId">Project Id</param>
        public void ValidateProjectId(string projectId)
        {
            ProjectIdConverter.ProjectIdToCampaignId(projectId);
        }

        /// <summary>
        /// Gets survey name according following template:
        /// 'survey name (p0000000)'.
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <returns>Formatted survey name.</returns>
        public static string GetFormattedSurveyName(int surveyId)
        {
            var survey = SurveyRepository.GetById(surveyId);
            return String.Format("{0} ({1})", survey.Description, survey.Name);
        }


        /// <summary>
        /// Gets survey name according following template:
        /// 'pxxxxx name_of_project'
        /// </summary>
        /// <param name="surveyId">Survey identifier.</param>
        /// <returns>Formatted survey name.</returns>
        public string GetProjectIdWithName(int surveyId)
        {
            var survey = SurveyRepository.GetById(surveyId);
            return String.Format("{0} {1}", survey.Name, survey.Description);
        }

        public void OnLaunchSurvey(int sid, Action<string> taskLog = null)
        {
            // if the company has telephony and survey is open pass actual parameters into dialer 
            if (!_mnTciTools.DoesCompanyUseTelephony())
            {
                return;
            }

            var surveyEntity = SurveyRepository.GetById(sid);
            if (surveyEntity.State == (int)SurveyState.Open)
            {
                taskLog?.Invoke("Updating survey settings on the dialer...");
                _telephony.SetCampaignParameters(surveyEntity.CampaignId, (DialingMode)surveyEntity.DialMode, surveyEntity.DialerParameters);
            }
        }

        public void OnDeleteSurvey(int sid)
        {
            _fcdQuotaService.OnDeleteSurvey(sid);
            _surveyMetadataCacheService.ResetSurveyCache(sid);
        }

        public BvScheduleEntity GetSchedule(int surveySid)
        {
            var survey = _surveyRepository.GetById(surveySid);

            return _scheduleRepository.GetById(survey.ScheduleID);
        }

        public List<CallHistoryDataEntity> GetCallHistoryData(string surveySIDs, DateTime? startTime, DateTime? endTime, string[] replicatedVariables)
        {
            var dataProvider = ServiceLocator.Resolve<ICallHistoryDataProvider>();

            return dataProvider.GetCallHistoryData(surveySIDs, startTime, endTime, replicatedVariables);
        }

        public DataTable GetCallsSentToDialerDistribution(int surveySid, DateTime? dateTime, int timezoneId, out int totalCount)
        {
            var result = new DataTable();

            using (IDataReader reader = BvSpGetCallsSentToDialerDistributionAdapter.ExecuteReader(dateTime, surveySid, timezoneId, out totalCount))
            {
                result.Load(reader);
            }

            return result;
        }

        public DataTable GetCallsDispositionCodes(int surveySid, DateTime startTime, DateTime endTime, out int totalCount)
        {
            var result = new DataTable();

            using (IDataReader reader = BvSpReportSampleStatusSummaryForDatesRangeAdapter.ExecuteReader(surveySid, startTime, endTime, out totalCount))
            {
                result.Load(reader);
            }

            return result;
        }

        public DataTable GetDialerCallsBreakdown(int surveySid, out int totalCount)
        {
            var result = new DataTable();

            using (IDataReader reader = BvSpGetDialerCallsBreakdownAdapter.ExecuteReader(surveySid, out totalCount))
            {
                result.Load(reader);
            }

            return result;
        }

        public void CleanupCallsDistribution(TimeSpan expirationPeriod)
        {
            var expirationDate = _timeService.GetUtcNow() - expirationPeriod;
            BvCallsSentToDialerAdapter.DeleteByCondition("Time < @Time", new SqlParameter("@Time", expirationDate));
        }

        public static void SetCallDeliveryMode(int surveyId, CallDeliveryMode callDeliveryMode)
        {
            using (var transaction = new DatabaseTransactionScope("SetCallDeliveryMode", DeadlockPriority.Supervisor))
            {
                BvSpSetCallDeliveryModeAdapter.ExecuteNonQuery(
                    surveyId, callDeliveryMode == CallDeliveryMode.Random);

                transaction.Commit();
            }
            
            ServiceLocator.Resolve<ISqlTableUpdatedPublisher>().PublishSurveyUpdated();
        }

        public static void UpdateLastTouchTime(int surveyId)
        {
            var survey = SurveyRepository.GetById(surveyId);
            survey.LastTouchTime = DateTime.UtcNow;
            SurveyRepository.Update(survey);
        }

        public BvSurveyEntity CreateSurvey(string confirmitProjectId, string confirmitSurveyName, string cfSqlServerConnectionString, string userName, string surveySqlServerName)
        {
            ValidateProjectId(confirmitProjectId);

            // If this survey already exists (we shouldn't throw exception here)
            BvSurveyEntity survey = _surveyRepository.TryGetByName(confirmitProjectId);
            if (survey != null)
            {
                Trace.TraceWarning("Couldn't add survey '{0}'. It is already exists.", survey.Name);
                return survey;
            }

            var callGroupSettings = ServiceLocator.Resolve<ISystemSettings>().CallGroup;
            var defaultSchedulingMode = SurveySchedulingMode.Normal;
            if (callGroupSettings.Enabled && callGroupSettings.EnabledForNewSurveys)
            {
                defaultSchedulingMode = SurveySchedulingMode.CallGroup;
            }

            var defaultDialerSurveyParameters = _dialerSurveyParametersManager.GetDialerDefaultSurveyParametersAsXml();

            survey = new BvSurveyEntity
            {
                Name = confirmitProjectId,
                DialMode = (byte)DialingMode.Manual,
                StateGroupID = StateGroupRepository.GetDefault().ID,
                Description = confirmitSurveyName,
                QuotaType = (byte)QuotaType.Pessimistic,
                CfDbSchemaPath = GetCfDbSchemaPath(cfSqlServerConnectionString),
                DialerParameters = defaultDialerSurveyParameters,
                SurveySchedulingMode = (short)defaultSchedulingMode,
                SurveySqlServerName = surveySqlServerName
            };

            int surveySid = _surveyRepository.Insert(survey);


            if (string.IsNullOrEmpty(userName) == false)
            {
                var callCenterService = ServiceLocator.Resolve<ICallCenterService>();
                var callCenter = callCenterService.GetSupervisorCallCenter(userName);

                if (callCenter != null)
                {
                    callCenterService.AssignSurvey(callCenter.ID, surveySid);
                }
            }

            SetCallDeliveryMode(surveySid, (CallDeliveryMode)_systemSettings.DefaultCallDeliveryMode);

            return _surveyRepository.GetById(survey.SID);
        }

        /// <summary>
        /// Gets the SQL schema path to Confirmit survey DB.
        /// Is a survey DB is on a remote server - it also checks that a linked server is created and available.
        /// </summary>
        /// <param name="cfSqlServerConnectionString">SQL connection string to survey DB.</param>
        /// <returns></returns>
        private static string GetCfDbSchemaPath(string cfSqlServerConnectionString)
        {
            var scsb = new SqlConnectionStringBuilder(cfSqlServerConnectionString);

            return $"[{scsb.InitialCatalog}].[dbo]";
        }

        public void UpdateReplicationScheme(BvSurveyEntity survey, TableInfo[] tables)
        {
            _replicationSchemaService.UpdateSurveyReplicationScheme(survey.SID, tables);

            if (tables != null && tables.Count() > 0)
            {
                UpdateReplicationStatus(survey.SID, true);
            }
        }

        public bool IsReplicationSchemaChanged(int surveySid, TableInfo[] tables)
        {
            return _replicationSchemaService.IsReplicationSchemaChanged(surveySid, tables);
        }



        /// <summary>
        /// Updates the survey replication status.
        /// </summary>
        /// <param name="surveySid">Survey SID</param>
        /// <param name="isReplicationEnabled">if set to <c>true</c> replication is enabled for survey.</param>
        /// <remarks>It sets ReplicationStatus flag in <c>BvSurvey</c> table.</remarks>
        public void UpdateReplicationStatus(int surveySid, bool isReplicationEnabled)
        {
            var survey = _surveyRepository.GetWithNoCache(surveySid);

            if (survey.ReplicationStatus != isReplicationEnabled)
            {
                survey.ReplicationStatus = isReplicationEnabled;
                _surveyRepository.Update(survey);
            }
        }

        public void CleanSurvey(int surveyId, CancellationToken cancellationToken)
        {
            CleanSurveyDataFromTable(surveyId, "BvAppointment", "SurveySID", cancellationToken);
            CleanSurveyDataFromTable(surveyId, "BvPersonDeferredMonitoring", "SurveySID", cancellationToken);
            CleanSurveyDataFromTable(surveyId, "BvSvySchedule", "SurveySID", cancellationToken);
            CleanSurveyDataFromTable(surveyId, "BvInterview", "SurveySID", cancellationToken);
            CleanSurveyDataFromTable(surveyId, "BvHistory", "SurveyID", cancellationToken);
            CleanSurveyDataFromTable(surveyId, "BvTimeBreaksHistory", "SurveyID", cancellationToken);
            CleanSurveyDataFromTable(surveyId, "BvCallHistory", "SurveyID", cancellationToken);
            CleanSurveyDataFromTable(surveyId, "BvCallHistoryEx", "SurveyID", cancellationToken);
            CleanSurveyDataFromTable(surveyId, "BvSamples", "SurveySID", cancellationToken);
            CleanReplicatedDataTable(surveyId);
            /*
             *  following tables should be clean automaticaly
             *  BvSampleStatusSummary through triggers on BvInterview and BvSampleStatusSummaryDelta
             *  BvInterviewQuotaCell through FK on BvInterview
             */

        }

        private static void CleanSurveyDataFromTable(int surveyId, string tableName, string columnNameOfsurveyId, CancellationToken cancellationToken)
        {
            int countOfDeletedItems;
            int batchSize = 10000;
            
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                countOfDeletedItems =
                    new DatabaseEngine().ExecuteScalar<int>(
                        String.Format(@"DELETE TOP( @Top ) FROM [{0}] WHERE {1} = @SurveyId; SELECT @@ROWCOUNT", tableName, columnNameOfsurveyId),
                        CommandType.Text,
                        new SqlParameter("@Top", batchSize),
                        new SqlParameter("@SurveyId", surveyId));
            } while (countOfDeletedItems > 0);

            EventDetailsScope.Current.AddTiming(String.Format("CleanSurveyDataFromTable({0})", tableName));
        }

        private static void CleanReplicatedDataTable(int surveyId)
        {
            new DatabaseEngine().ExecuteNonQuery($"TRUNCATE TABLE BvReplicatedData_{surveyId}", CommandType.Text);
        }
        
        public void UpdateQuotaBalancingConfiguration(int surveySid, TableInfo[] tables)
        {
            _replicationSchemaService.UpdateQuotaBalancingConfiguration(surveySid, tables);
        }
    }
}
