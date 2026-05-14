using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Services
{
    public class CallsManagementService : ICallsManagementService
    {
        private readonly IRemoteDataCopier _remoteDataCopier;
        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly IDatabaseEngineFactory _databaseEngineFactory;
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;
        private readonly IEditCallsQueryProvider _editCallsQueryProvider;
        private readonly IFCDSettings _fcdSettings;

        public CallsManagementService(IRemoteDataCopier remoteDataCopier,
            ISurveyConnectionStringProvider surveyConnectionStringProvider,
            IDatabaseEngineFactory databaseEngineFactory,
            ISurveyDatabaseEngine surveyDatabaseEngine,
            IEditCallsQueryProvider editCallsQueryProvider,
            IFCDSettings fcdSettings)
        {
            _remoteDataCopier = remoteDataCopier;
            _surveyConnectionStringProvider = surveyConnectionStringProvider;
            _databaseEngineFactory = databaseEngineFactory;
            _surveyDatabaseEngine = surveyDatabaseEngine;
            _editCallsQueryProvider = editCallsQueryProvider;
            _fcdSettings = fcdSettings;
        }

        public void Activate(
            int? SurveySID,
            int? Mode,
            int? BatchID,
            int? Priority,
            int? PersonSID,
            int? ShiftTypeID,
            DateTime? TimeToCall,
            bool? EnableDisabledCalls,
            int? DefaultTZID,
            int? ITS)
        {
            using (var connectionScope = new ConnectionScope())
            {
                var localTempTableName = "#InterviewIts";
                var createTempTableQuery = $"CREATE TABLE {localTempTableName} (Id INT, its SMALLINT)";
                _databaseEngineFactory.CreateForCurrentInstanceDatabase().ExecuteNonQuery(createTempTableQuery, CommandType.Text);

                BvSpCall_ActivateAdapter.ExecuteNonQuery(
                    SurveySID,
                    Mode,
                    BatchID,
                    Priority,
                    PersonSID,
                    ShiftTypeID,
                    TimeToCall,
                    EnableDisabledCalls,
                    DefaultTZID,
                    ITS,
                    out int processedCalls);

                if (processedCalls == 0 || !SurveySID.HasValue)
                {
                    return;
                }

                var copyDataQuery = $"SELECT * from {localTempTableName}";

                var remoteTempTableName = "#RemoteInterviewIts";
                var query = $@"
                    UPDATE <Schema>.response_control 
                    SET ITS = ii.its
                    FROM {remoteTempTableName} as ii 
                    WHERE respid = ii.Id";

                var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(SurveySID.Value);
                using (var remoteConnectionProvider = new RemoteConnectionProvider(surveyConnectionInfo.ConnectionString))
                {
                    _remoteDataCopier.CopyDataToNewTable(
                        connectionScope, remoteConnectionProvider, remoteTempTableName, copyDataQuery, surveyConnectionInfo.SchemaName);

                    _surveyDatabaseEngine.ExecuteNonQuery(remoteConnectionProvider.Connection, SurveySID.Value, query);
                }
            }
        }

        public void Edit(
            int surveySid,
            int batchId,
            DateTime? timeToCall,
            DateTime? timeToExpire,
            int? callState,
            int? callPriority,
            int? shiftType,
            int? extendedStatus,
            byte? dialingMode)
        {
            using (var connectionScope = new ConnectionScope())
            {
                var localTempTableName = "#InterviewIds";

                IDatabaseEngine dbEngine = _databaseEngineFactory.CreateForCurrentInstanceDatabase();
                var createTempTableQuery = $"CREATE TABLE {localTempTableName} (Id INT)";
                dbEngine.ExecuteNonQuery(createTempTableQuery, CommandType.Text);

                string whereCondition = string.Empty;
                int stateGroupId = 0;
                var survey = SurveyRepository.GetById(surveySid);
                if (callState.HasValue)
                {
                    stateGroupId = survey.StateGroupID;
                }

                var query = _editCallsQueryProvider.GetQuery(
                    surveySid,
                    batchId,
                    timeToCall,
                    timeToExpire,
                    callState,
                    callPriority,
                    shiftType,
                    extendedStatus,
                    dialingMode,
                    _fcdSettings.BehaviorType,
                    whereCondition,
                    stateGroupId);

                var parameters = _editCallsQueryProvider.GetSqlParameters(
                    surveySid,
                    batchId,
                    timeToCall,
                    timeToExpire,
                    callState,
                    callPriority,
                    shiftType,
                    extendedStatus,
                    dialingMode,
                    _fcdSettings.BehaviorType,
                    stateGroupId);

                var processedCalls = new DatabaseEngine().ExecuteScalar<int>(query, CommandType.Text, parameters.ToArray());

                if (extendedStatus.HasValue && processedCalls > 0)
                {
                    UpdateResponseControlAfterItsChange(connectionScope, localTempTableName, extendedStatus, surveySid);
                }
            }
        }

        public void MoveToIts(
            int? surveySid,
            int? batchId,
            int? stateId)
        {
            using (var connectionScope = new ConnectionScope())
            {
                var localTempTableName = "#InterviewIds";
                var createTempTableQuery = $"CREATE TABLE {localTempTableName} (Id INT, DialingMode TINYINT, its SMALLINT)";
                _databaseEngineFactory.CreateForCurrentInstanceDatabase().ExecuteNonQuery(createTempTableQuery, CommandType.Text);

                BvSpCall_MoveToITSAdapter.ExecuteNonQuery(
                    surveySid,
                    batchId,
                    stateId,
                    out int processedCalls);

                if (processedCalls == 0 || !surveySid.HasValue)
                {
                    return;
                }

                UpdateResponseControlAfterItsChange(connectionScope, localTempTableName, stateId, surveySid.Value);
            }
        }

        private void UpdateResponseControlAfterItsChange(ConnectionScope connectionScope, string localTempTableName, int? stateId, int surveySid)
        {
            var copyDataQuery = $"SELECT * from {localTempTableName}";

            var remoteTempTableName = "#RemoteInterviewIds";
            var query = $@"
                    UPDATE <Schema>.response_control 
                    SET ITS = cast({stateId} as nvarchar(10))
                    FROM {remoteTempTableName} as ids 
                    WHERE respid = ids.ID";

            var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveySid);
            using (var remoteConnectionProvider = new RemoteConnectionProvider(surveyConnectionInfo.ConnectionString))
            {
                _remoteDataCopier.CopyDataToNewTable(
                    connectionScope, remoteConnectionProvider, remoteTempTableName, copyDataQuery, surveyConnectionInfo.SchemaName);

                _surveyDatabaseEngine.ExecuteNonQuery(remoteConnectionProvider.Connection, surveySid, query);
            }
        }

        public List<CallInfo> GetCallsToFlushOnDialer(int surveyId, int batchId, bool isRecording)
        {
            //TODO CODI changes: get isRecording flag form the surevey properties

            const string queryToGetCalls = @"
SELECT c.ID,
c.ExplicitSID AS ExplicitSid,
c.SurveySID AS SurveySid,
i.DialingMode DiallingMode,
c.InterviewID AS InterviewID,
i.TelephoneNumber,
i.ExtensionNumber,
c.TimeInShift,
0 as GroupId -- Later we might have GroupId here somehow 
FROM BvTransferArrays iids 
INNER JOIN BvInterview i ON i.ID = iids.ItemID AND i.SurveySid = {0}
INNER JOIN BvSvySchedule c ON c.InterviewID = iids.ItemID
WHERE c.SurveySid = {0} AND c.CallState = -2 AND iids.BatchID = {1}
";
            using (IDataReader dataReader = new DatabaseEngine().ExecuteReaderInNewConnection(
                String.Format(queryToGetCalls, surveyId, batchId), CommandType.Text))
            {
                return ReadFlushedCallInfos(isRecording, dataReader);

            }
        }

        public List<CallInfo> ReadFlushedCallInfos(bool isRecording, IDataReader dataReader)
        {
            var result = new List<CallInfo>();
            while (dataReader.Read())
            {
                var agentId = (int)dataReader["ExplicitSid"];

                var timeToCall = (DateTime?)dataReader["TimeInShift"];

                if ((timeToCall != null) && (timeToCall.Equals(CallDeliveryService.NullTime)))
                {
                    timeToCall = null;
                }

                // ExtensionNumber field is being used for storing Caller ID
                var callerIdOrdinal = dataReader.GetOrdinal("ExtensionNumber");
                var callerId = dataReader.IsDBNull(callerIdOrdinal) ? string.Empty : dataReader.GetString(callerIdOrdinal);

                result.Add(new CallInfo(
                    agentId,
                    ((int)dataReader["InterviewID"]),
                    (long)(int)dataReader["ID"], //TODO CODI changes: propagate callId 'long' type to the CATI DB
                    (int)dataReader["GroupID"],
                    (string)dataReader["TelephoneNumber"],
                    timeToCall,
                    (byte)dataReader["DiallingMode"] == 0
                        ? DialingMode.Predictive
                        : (DialingMode)Convert.ToInt32(dataReader["DiallingMode"]), // Its means 
                                                                                    // that all calls have DialingMode = Predictive by default, we mean FlushNumbers always called for Predictive surveys.
                    false, //'wasAbandoned' must be taken from call in fact
                    0, //'attemptsMade' must be taken from call in fact
                    0, // 'previousConnects' must be taken from call in fact
                    0, // 'numberOfNoAnswer' must be taken from call in fact'
                    "",
                    isRecording,
                    0,
                    callerId,
                    null));
            }
            return result;
        }

        public void RemoveFilteredCalls(
            int surveyId,
            int batchId,
            int? newIts)
        {
            const string interviewIdsTempTable = "#interviewIds";
            var catiDbQuery = GenerateCatiDbQuery(surveyId, batchId, newIts, interviewIdsTempTable);
            var surveyDbQuery = GenerateSurveyDbQuery(newIts, interviewIdsTempTable);

            using (var transaction = new DatabaseTransactionScope("OnQuotaCloseCells"))
            {
                using (var connectionScope = new ConnectionScope())
                {
                    new DatabaseEngine().ExecuteNonQuery(catiDbQuery, CommandType.Text);

                    if (!string.IsNullOrWhiteSpace(surveyDbQuery))
                    {
                        var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId);
                        var copyDataQuery = $"SELECT * FROM [{interviewIdsTempTable}]";

                        using (var surveyConnectionProvider = new RemoteConnectionProvider(surveyConnectionInfo.ConnectionString))
                        {
                            _remoteDataCopier.CopyDataToNewTable(
                                connectionScope, surveyConnectionProvider, interviewIdsTempTable, copyDataQuery, surveyConnectionInfo.SchemaName);

                            _surveyDatabaseEngine.ExecuteNonQuery(surveyConnectionProvider.Connection, surveyId, surveyDbQuery);
                        }
                    }
                }

                transaction.Commit();
            }
        }

        private static string GenerateCatiDbQuery(int surveyId, int batchId, int? newIts, string tempTable)
        {
            var catiDbQuery = new StringBuilder();

            catiDbQuery.AppendLine($@"
                CREATE TABLE {tempTable} (
                    [InterviewId]  INT NOT NULL
                );

                UPDATE BvSvySchedule
                SET CallState = 0
                OUTPUT deleted.InterviewID
                INTO {tempTable} (InterviewId)
                FROM BvTransferArrays iids
                WHERE iids.BatchID = {batchId} AND 
                    iids.ItemID = BvSvySchedule.InterviewID AND
                    (CallState > 0 ) AND surveySID = {surveyId}

                UPDATE BvAppointment
                SET STATE = 2
                FROM {tempTable} ta
                WHERE BvAppointment.SurveySID = {surveyId} AND
	                  BvAppointment.InterviewSID = ta.InterviewId 
            ");

            if (newIts != null)
            {
                catiDbQuery.AppendLine($@"
                    UPDATE BvInterview
                    SET TransientState = {newIts}
                    FROM {tempTable} ta
                    WHERE SurveySid = {surveyId} AND
                          ta.InterviewId = BvInterview.ID
                ");
            }

            return catiDbQuery.ToString();
        }

        private string GenerateSurveyDbQuery(int? newIts, string tempTable)
        {
            var surveyDbQuery = new StringBuilder();
            if (newIts != null)
            {
                surveyDbQuery.AppendLine($@"
                    UPDATE <Schema>.response_control
                    SET ITS = {newIts} 
                    FROM <Schema>.{tempTable} as ids 
                    WHERE respid = ids.InterviewId
                ");
            }
            return surveyDbQuery.ToString();
        }
    }
}
