using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.InterviewServiceImplementation;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;
using Confirmit.CATI.Core.Resources;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;

namespace Confirmit.CATI.Core.Services.SurveyArchiveServiceImplementation
{
    public class SurveyArchiveService : ISurveyArchiveService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IReplicationService _replicationService;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleService _scheduleService;
        private readonly ISystemSettings _systemSettings;
        private readonly IStateGroupService _stateGroupService;
        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly IRemoteDataCopier _remoteDataCopier;
        private readonly IInterviewQuotaCellService _interviewQuotaCellService;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;

        public class SurveyArchive
        {
            public class SchedulingScriptData
            {
                [XmlAttribute("Id")]
                public int Id;

                [XmlAttribute("Name")]
                public string Name;

                [XmlElement("Source")]
                public string Source;
            }

            public class StateGroupData
            {
                public class StateData
                {
                    [XmlAttribute("Its")]
                    public int Its;

                    [XmlAttribute("Name")]
                    public string Name;

                    [XmlAttribute("Priority")]
                    public int Priority;

                    [XmlAttribute("Disable")]
                    public bool Disable;
                }

                [XmlAttribute("Id")]
                public int Id;

                [XmlAttribute("Name")]
                public string Name;

                [XmlArray("State")]
                public List<StateData> States;
            }

            [XmlElement("SchedulingScript")]
            public SchedulingScriptData SchedulingScript;

            [XmlElement("StateGroup")]
            public StateGroupData StateGroup;
        }

        public SurveyArchiveService(
            ISurveyRepository surveyRepository,
            IReplicationService replicationService,
            IScheduleRepository scheduleRepository,
            IScheduleService scheduleService,
            ISystemSettings systemSettings,
            IStateGroupService stateGroupService,
            ISurveyConnectionStringProvider surveyConnectionStringProvider,
            IRemoteDataCopier remoteDataCopier,
            IInterviewQuotaCellService interviewQuotaCellService,
            ISqlTableUpdatedPublisher sqlTableUpdatedPublisher)
        {
            _surveyRepository = surveyRepository;
            _replicationService = replicationService;
            _scheduleRepository = scheduleRepository;
            _scheduleService = scheduleService;
            _systemSettings = systemSettings;
            _stateGroupService = stateGroupService;
            _surveyConnectionStringProvider = surveyConnectionStringProvider;
            _remoteDataCopier = remoteDataCopier;
            _interviewQuotaCellService = interviewQuotaCellService;
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
        }

        public string Archive(BvSurveyEntity survey)
        {
            var evt = new BackupSurveyToArchiveEvent(survey.SID, survey.Name);

            var schedule = _scheduleRepository.GetById(survey.ScheduleID);
            var group = StateGroupRepository.GetById(survey.StateGroupID);
            var states = StateRepository.GetAll(survey.StateGroupID);

            var archive = new SurveyArchive
            {
                SchedulingScript =
                    new SurveyArchive.SchedulingScriptData
                    {
                        Id = schedule.ScheduleID,
                        Name = schedule.Name,
                        Source = schedule.XmlUnderDev
                    },
                StateGroup =
                    new SurveyArchive.StateGroupData()
                    {
                        Id = group.ID,
                        Name = group.Name,
                        States =
                            states.Select(
                                    x =>
                                        new SurveyArchive.StateGroupData.StateData
                                        {
                                            Its = x.StateID,
                                            Name = x.Name,
                                            Priority = x.Priority,
                                            Disable = x.DA != 0
                                        })
                                .ToList()
                    }
            };

            var sw = new StringWriter();

            new XmlSerializer(typeof(SurveyArchive)).Serialize(sw, archive);

            evt.Finish();

            return sw.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public string Restore(int surveyId, string data, CancellationToken cancellationToken)
        {
            var warning = new List<string>();

            var survey = _surveyRepository.GetById(surveyId);

            if (survey.State == (int)SurveyState.SoftDeleted)
            {
                survey.State = (int)SurveyState.Close;
                BvSurveyAdapter.Update(survey);
                _sqlTableUpdatedPublisher.PublishSurveyUpdated();
                return null;
            }

            var sr = new StringReader(data);
            var archive = (SurveyArchive)new XmlSerializer(typeof(SurveyArchive)).Deserialize(sr);

            CreateSchedulingScriptIfNeed(survey, warning, archive);

            SetStateGroup(survey, warning, archive);

            RestoreInterviewData(survey);

            _replicationService.RereadSurveyReplicatedData(survey.SID, "Restore Surveys", cancellationToken);

            _interviewQuotaCellService.Populate(survey.SID, cancellationToken);

            if (warning.Count != 0)
            {
                return string.Format(Strings.SurveyArchive_SurveyWasRestoredWithWarn, string.Join(System.Environment.NewLine, warning));
            }

            return string.Empty;
        }

        private void RestoreInterviewData(BvSurveyEntity survey)
        {
            var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(survey.SID);
            var schema = surveyConnectionInfo.SchemaName;
            var existsBatchId = new DatabaseEngine(surveyConnectionInfo.ConnectionString).ExecuteScalar<int>(
                                    $"SELECT ISNULL( (SELECT 1 FROM syscolumns c WHERE  c.Name = 'batchId' AND c.Id = OBJECT_ID('{schema}.respondent')), 0 )",
                                    CommandType.Text) != 0;

            if (!existsBatchId)
            {
                return;
            }

            using (var transaction = new DatabaseTransactionScope("Archive.RestoreInterview"))
            {
                bool isExistsDialTypeColumns = RespondentDataObtainer.IsColumnExists(surveyConnectionInfo, "respondent", "DialType");

                var dialTypeColumn = isExistsDialTypeColumns ? "ISNULL(r.DialType, 0)" : "0";

                var tempTableName = "#TempTableName";

                // Ignore rows with the same [respid] field in [response_control] table.
                // Get the first row in this case
                var copyDataQuery = $@"
                    SELECT 
                        r.respid
                        ,{survey.SID} as SurveySid
                        ,r.TelephoneNumber
                        ,r.RespondentName
                        ,r.TimeZoneId
                        ,ISNULL( rc.its, 16 ) as TransientState
                        ,NULL as LastCallTime
                        ,0 as LastCallPersonSID
                        ,0 as Duration
                        ,r.ExtensionNumber
                        ,r.sid
                        ,r.batchId
                        ,ISNULL( r.LastChannelId, 0 ) as LastChannelID
                        ,ISNULL( r.DialMode, 1 ) as DialingMode
                        ,{dialTypeColumn} as DialTypeId                    
                    FROM <Schema>.respondent r LEFT JOIN ( 
                         SELECT *, ROW_NUMBER() OVER (PARTITION BY [respid] ORDER BY [respid]) AS row_num
                         FROM <Schema>.response_control
                    ) rc ON r.respid = rc.respid 
                    WHERE r.batchId IS NOT NULL AND (rc.row_num = 1 OR rc.row_num IS NULL)";

                var query = $@"
                    INSERT INTO [BvInterview]
                        ([ID]
                        ,[SurveySID]
                        ,[TelephoneNumber]
                        ,[RespondentName]
                        ,[TimezoneID]
                        ,[TransientState]
                        ,[LastCallTime]
                        ,[LastCallPersonSID]
                        ,[Duration]
                        ,[ExtensionNumber]
                        ,[ConfirmitSid]
                        ,[BatchID]
                        ,[LastChannelID]
                        ,[DialingMode]
                        ,[DialTypeId])
                    SELECT * FROM {tempTableName}";

                using (var connectionScope = new ConnectionScope())
                {
                    _remoteDataCopier.CopyDataToNewTable(
                        surveyConnectionInfo.ConnectionString, connectionScope, tempTableName, copyDataQuery, surveyConnectionInfo.SchemaName);

                    new DatabaseEngine().ExecuteNonQueryWithSpecificTimeOut(
                        query, CommandType.Text, (int)_systemSettings.AsyncOperation.RestoreSurveySqlTimeout.TotalSeconds);
                }

                transaction.Commit();
            }
        }

        private void SetStateGroup(BvSurveyEntity survey, List<string> warning, SurveyArchive archive)
        {
            var stateGroup = StateGroupRepository.GetById(archive.StateGroup.Id);
            if (stateGroup != null)
            {
                if (!CompareArchivedStateGroupWithDbStateGroup(archive.StateGroup))
                {
                    warning.Add(Strings.SurveyArchive_Warn_StateGroupWasChanged);
                }

                survey.StateGroupID = stateGroup.ID;
            }
            else
            {
                survey.StateGroupID = StateGroupRepository.GetDefault().ID;
            }

            using (var transaction = new DatabaseTransactionScope("Archive.RestoreState"))
            {
                _surveyRepository.Update(survey);
                transaction.Commit();
            }
        }

        private static bool CompareArchivedStateGroupWithDbStateGroup(SurveyArchive.StateGroupData group)
        {
            var dbGroup = StateGroupRepository.GetById(group.Id);
            if (dbGroup.Name != group.Name)
            {
                return false;
            }

            var states = StateRepository.GetAll(group.Id);
            if (group.States.Count != states.Count)
            {
                return false;
            }

            var itsToStates = states.ToDictionary(x => x.StateID);
            foreach (var state in group.States)
            {
                BvStateEntity dbState;
                if (!itsToStates.TryGetValue(state.Its, out dbState))
                {
                    return false;
                }
                if (dbState.Name != state.Name ||
                    dbState.Priority != state.Priority ||
                    (dbState.DA == 1) != state.Disable)
                {
                    return false;
                }
            }

            return true;
        }

        private void CreateSchedulingScriptIfNeed(BvSurveyEntity survey, List<string> warning, SurveyArchive archive)
        {
            var schedule = _scheduleRepository.GetById(archive.SchedulingScript.Id);
            if (schedule != null)
            {
                if (schedule.XmlUnderDev != archive.SchedulingScript.Source)
                {
                    warning.Add(Strings.SurveyArchive_Warn_SchedulingScriptWasChanged);
                }

                survey.ScheduleID = schedule.ScheduleID;
                _surveyRepository.Update(survey);
            }
            else
            {
                using (var transaction = new DatabaseTransactionScope("Archive.RestoreScript", DeadlockPriority.Supervisor))
                {
                    var scheduleId =
                        _scheduleRepository.InsertWithSpecificId(
                            new BvScheduleEntity
                            {
                                ScheduleID = archive.SchedulingScript.Id,
                                Name = archive.SchedulingScript.Name,
                                XmlUnderDev = archive.SchedulingScript.Source.Trim()
                            });

                    _scheduleService.Launch(scheduleId);

                    survey.ScheduleID = scheduleId;
                    _surveyRepository.Update(survey);

                    transaction.Commit();
                }
            }
        }
    }
}
