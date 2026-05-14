using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Common;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Services.Survey.Quota;

namespace Confirmit.CATI.Core.Services.ReplicationServiceImplementation
{
    /// <summary>
    /// Class contains methods related to data replication process from CF to CATI.
    /// </summary>
    public class ReplicationService : IReplicationService
    {
        private const string ReplicationTablePrimaryKey = "respid";

        /// <summary>
        /// Timeout (in seconds) for the <see cref="SqlCommand"/> used to run the replication process.
        /// </summary>
        private const int ReplicationSqlCommandTimeout = 900; // 15 minutes
        private const int BatchSize = 10000;
        private const int InvalidColumnNameSqlExceptionNumber = 207;

        private readonly IQuotaClusteringSyncService _quotaClusteringSyncService;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IReplicationSchemaInfoService _replicationSchemaInfoService;
        private readonly ISurveyConnectionStringProvider _surveyConnectionStringProvider;
        private readonly IRemoteDataCopier _remoteDataCopier;
        private readonly IReplicationIndexService _replicationIndexService;
        private readonly IInterviewQuotaCellService _interviewQuotaCellService;
        private readonly QuotaMatcherBuilder _quotaMatcherBuilder;
        private readonly IReplicatedDataRepository _replicatedDataRepository;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;
        private readonly IProjectsActivityService _projectsActivityService;
        
        public ReplicationService(
            IQuotaClusteringSyncService quotaClusteringSyncService,
            ISurveyRepository surveyRepository,
            IReplicationSchemaInfoService replicationSchemaInfoService,
            ISurveyConnectionStringProvider surveyConnectionStringProvider,
            IRemoteDataCopier remoteDataCopier,
            IReplicationIndexService replicationIndexService,
            IInterviewQuotaCellService interviewQuotaCellService,
            QuotaMatcherBuilder quotaMatcherBuilder,
            IReplicatedDataRepository replicatedDataRepository,
            ISqlTableUpdatedPublisher sqlTableUpdatedPublisher, 
            IProjectsActivityService projectsActivityService)
        {
            _quotaClusteringSyncService = quotaClusteringSyncService;
            _surveyRepository = surveyRepository;
            _replicationSchemaInfoService = replicationSchemaInfoService;
            _surveyConnectionStringProvider = surveyConnectionStringProvider;
            _remoteDataCopier = remoteDataCopier;
            _replicationIndexService = replicationIndexService;
            _interviewQuotaCellService = interviewQuotaCellService;
            _quotaMatcherBuilder = quotaMatcherBuilder;
            _replicatedDataRepository = replicatedDataRepository;
            _sqlTableUpdatedPublisher = sqlTableUpdatedPublisher;
            _projectsActivityService = projectsActivityService;
        }

        /// <summary>
        /// Runs the replication. Does not return control till the end of replication
        /// process. It controls the frequency of method calls so some method calls may not
        /// result in replication process if it is called to early after the previous
        /// replication. If there are some other replication
        /// process currently executing - it will immediately return the control.
        /// </summary>
        public void RunPeriodicalReplication(CancellationToken cancellationToken)
        {
            RunReplication(
                0,
                new ExclusiveDatabasePeriodicalLockFactory(
                    "ReplicationService.RunPeriodicalReplication",
                    ServiceLocator.Resolve<ISystemSettings>().Replication.BackgroundReplicationSleepPeriod),
                new DatabaseTransactionOptions("RunReplication", DeadlockPriority.PeriodicalThread), cancellationToken);
        }

        /// <summary>
        /// Runs the replication. Does not return control till the end of replication
        /// process. It does not control the frequency of method calls so all method calls
        /// will result in replication process. If there are some other replication
        /// process currently executing - it will wait till the end of it before start.
        /// TODO: Used in the tests only, remove
        /// </summary>
        public void RunForceReplication()
        {
            var lockFactory = new ExclusiveDatabaseLockFactory(
                "ReplicationService.RunForceReplication",
                ServiceLocator.Resolve<ISystemSettings>().Replication.ForceReplicationLockTimeout);

            RunReplication(0, lockFactory, new DatabaseTransactionOptions("RunForceReplication"));
        }

        /// <summary>
        /// TODO: Need to clarify where UpdateSurveyReplicationScheme is called from and WHY
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="cancellationToken"></param>
        public void RunForceReplication(int surveyId, CancellationToken cancellationToken)
        {
            var lockFactory = new ExclusiveDatabaseLockFactory(
                "ReplicationService.RunForceReplication",
                ServiceLocator.Resolve<ISystemSettings>().Replication.ForceReplicationLockTimeout);

            RunReplication(surveyId, lockFactory, new DatabaseTransactionOptions("RunForceReplication"), cancellationToken);
        }

        /// <summary>
        /// The method is called in the begin of sample addition.
        /// At the moment records in the replication tables added ONLY where and NOT added during regular replication.
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="batchId"></param>
        /// <param name="cancellationToken"></param>
        public void UploadSampleDataToReplicatedTable(int surveyId, int batchId, CancellationToken cancellationToken)
        {
            var tableName = ReplicationSchemaService.GetDestinationTableName(surveyId);
            CopyReplicationDataToTable(surveyId, tableName, batchId, 0, cancellationToken);
        }

        /// <summary>
        /// The function used in the survey launch and restore operations where we do reread all data
        /// </summary>
        public void RereadSurveyReplicatedData(int surveyId, string reason, CancellationToken cancellationToken)
        {
            EventDetailsScope.Current.AddTiming("RereadSurveyReplicatedData.Begin");

            using (var dbLock = DatabaseLockService.CreateLock(
                DatabaseLockTimeoutsAndRecourceNames.GetSurveyReplicationResourceName(surveyId),
                "ReplicationService.RereadSurveyReplicatedData",
                ServiceLocator.Resolve<ISystemSettings>().Replication.ForceReplicationLockTimeout))
            {
                dbLock.EnterLock();

                // Versions are tracked per database
                var surveyDatabaseVersion = GetSurveyDatabaseChangeTrackingVersion(surveyId);

                RereadSurveyReplicatedDataInternal(surveyId, surveyDatabaseVersion, reason, cancellationToken);
            }
        }

        /// <summary>
        /// The function used in the survey launch and restore operations where we do reread all data and in the periodical replication if we detect there is something wrong with replication versiond
        /// </summary>
        public void RereadSurveyReplicatedDataInternal(int surveyId, long surveyDatabaseVersion, string replicationReason, CancellationToken cancellationToken)
        {
            var evt = new RereadReplicationEvent();

            evt.Details.Messages.Add(replicationReason);
            evt.Details.Messages.Add("Survey Database Version: " + surveyDatabaseVersion);

            evt.ObjectId = surveyId;
            evt.ObjectName = SurveyRepository.GetById(surveyId).Name;

            using (new EventDetailsScope(evt.Details))
            {
                EventDetailsScope.Current.AddTiming("RereadSurveyReplicatedDataInternal.Begin");

                var curTableName = ReplicationSchemaService.GetDestinationTableName(surveyId);
                var tmpTableName = curTableName + "_tmp_" + Guid.NewGuid().ToString().Replace('-', '_');
                var delTableName = curTableName + "_del_" + Guid.NewGuid().ToString().Replace('-', '_');

                var cleanQuery =
                    String.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP TABLE [{0}]" +
                                  "IF OBJECT_ID('{1}') IS NOT NULL DROP TABLE [{1}]",
                        tmpTableName, delTableName);

                var dbEngine = new DatabaseEngine();
                try
                {
                    //create empty table without indexes
                    _replicationSchemaInfoService.CreateCopyOfTableWithoutDataAndIndexes(curTableName, tmpTableName,
                        out var indexQueries);
                    EventDetailsScope.Current.AddTiming(
                        "RereadSurveyReplicatedData: CreateCopyOfTableWithoutDataAndIndexes");

                    // Copy data to table
                    var recordsCopied = CopyReplicationDataToTable(surveyId, tmpTableName, 0, 0, cancellationToken, SqlBulkCopyOptions.FireTriggers);

                    EventDetailsScope.Current.AddTiming("RereadSurveyReplicatedData: CopyReplicatedDataToTable");
                    evt.Details.Messages.Add("Records copied: " + recordsCopied);

                    // Create clustered index
                    var clusteredIndexQuery = indexQueries.FirstOrDefault();
                    if (clusteredIndexQuery != null)
                    {
                        dbEngine.ExecuteNonQueryWithSpecificTimeOut(clusteredIndexQuery, CommandType.Text, ReplicationSqlCommandTimeout);
                    }

                    EventDetailsScope.Current.AddTiming("RereadSurveyReplicatedData: Create clustered index");

                    DeleteWebInterviews(surveyId, tmpTableName);

                    EventDetailsScope.Current.AddTiming("RereadSurveyReplicatedData: Delete web interviews");

                    // Create non-clustered indexes
                    foreach (var indexQuery in indexQueries.Skip(1))
                    {
                        dbEngine.ExecuteNonQueryWithSpecificTimeOut(indexQuery, CommandType.Text, ReplicationSqlCommandTimeout);
                    }

                    EventDetailsScope.Current.AddTiming("RereadSurveyReplicatedData: Create non-clustered indexes");

                    var createRespondentTriggerQuery = GetUpdateRespondentTriggerQuery(
                        dbEngine, curTableName, tmpTableName, surveyId, out string dropTriggersQuery);
                    //swap tables and update LastVersion
                    using (var transaction = new DatabaseTransactionScope(new DatabaseTransactionOptions("RereadSurveyReplicatedData")))
                    {
                        var query = $@"EXEC sp_rename '{curTableName}', '{delTableName}';
                            EXEC sp_rename '{tmpTableName}', '{curTableName}';
                            {dropTriggersQuery}";
                        dbEngine.ExecuteBatch(query);

                        dbEngine.ExecuteBatch(createRespondentTriggerQuery);

                        query = $"UPDATE BvReplicationTables set LastVersion = {surveyDatabaseVersion} where SurveySid= {surveyId}";
                        dbEngine.ExecuteBatch(query);

                        transaction.Commit();
                    }

                    EventDetailsScope.Current.AddTiming("RereadSurveyReplicatedData: Swap tables");
                }
                catch (Exception e)
                {
                    TraceHelper.TraceException(e, "RereadSurveyReplicatedDataInternal");
                    throw;
                }
                finally
                {
                    Cleanup(cleanQuery);
                    EventDetailsScope.Current.AddTiming("RereadSurveyReplicatedData: Execute clean query");
                }
            } // using (new EventDetailsScope(evt.Details))

            evt.Finish();
        }

        private string GetUpdateRespondentTriggerQuery(
            DatabaseEngine dbEngine, string curTableName, string tmpTableName, int surveyId, out string dropTriggersQuery)
        {
            string tmpTriggerName = _replicationIndexService.GetNameOfRespondentUpdateTrigger(tmpTableName);
            string correctTriggerName = _replicationIndexService.GetNameOfRespondentUpdateTrigger(curTableName);

            string query = $@"SELECT COUNT(*) 
                FROM sys.triggers
                WHERE [name] = '{tmpTriggerName}'";

            int cnt = dbEngine.ExecuteScalar<int>(query);

            if (cnt == 0)
            {
                dropTriggersQuery = string.Empty;
                return string.Empty;
            }

            dropTriggersQuery = $@"DROP TRIGGER [{tmpTriggerName}];
                DROP TRIGGER[{ correctTriggerName}];";

            return $@"CREATE TRIGGER [{correctTriggerName}] ON {curTableName} AFTER INSERT, UPDATE 
                AS
                BEGIN
                    {_replicationIndexService.GetBodyOfRespondentUpdateTrigger(surveyId)}
                END;";
        }

        private static void Cleanup(string cleanupQuery)
        {
            try
            {
                new DatabaseEngine().ExecuteNonQueryInNewConnection(cleanupQuery, ReplicationSqlCommandTimeout);
            }
            catch (Exception e)
            {
                TraceHelper.TraceException(e);
            }
        }

        private static string GenerateDeleteWebInterviewsQuery(int surveyId, string table2DeteleInterviews)
        {
            return string.Format(
                "      DELETE FROM [{0}] FROM [{0}] LEFT JOIN [BvInterview] ON [BvInterview].[SurveySID] = {1} AND [BvInterview].[ID] = [{0}].[{2}] WHERE [BvInterview].[ID] IS NULL",
                table2DeteleInterviews,
                surveyId,
                ReplicationTablePrimaryKey);
        }

        private static void DeleteWebInterviews(int surveyId, string table2DeteleInterviews)
        {
            var query = GenerateDeleteWebInterviewsQuery(surveyId, table2DeteleInterviews);

            new DatabaseEngine().ExecuteNonQuery(query, CommandType.Text);
        }

        private int CopyReplicationDataToTable(int surveyId, string tableName, int batchId, int respId, CancellationToken cancellationToken, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
        {
            var query = "";

            try
            {
                var tables =
                    BvReplicationTablesAdapter.GetByCondition("SurveySid = @SurveyID",
                        new SqlParameter("@SurveyID", surveyId))
                        .OrderBy(x => x.TableName != "respondent").ToArray();

                if (!tables.Any())
                {
                    return 0;
                }

                query = ReplicationSchemaService.GetSelectForReplicatedDataTable(batchId, respId, tables);

                return BulkInsertReplicatedDataToTable(surveyId, tableName, query, options, cancellationToken);
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "Error during CopyReplicatedDataToTable for surveyId ={0}, batchId = {1}, Query: {2}, Exception: {3}",
                    surveyId, batchId, query, ex);

                throw;
            }
        }


        private int BulkInsertReplicatedDataToTable(int surveyId, string destinationTableName, string query, SqlBulkCopyOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveyId, updateLastConnectionTime: false);
            int recordsCopied;
            
            using (var connectionScope = new ConnectionScope())
            {
                if (IsPopulatingBvInterviewQuotaCellTableRequired(surveyId, destinationTableName))
                {
                    var quotaMatcher = _quotaMatcherBuilder.Build(surveyId);

                    recordsCopied = _remoteDataCopier.CopyDataToExistTableWithCallback(
                            surveyConnectionInfo.ConnectionString,
                            connectionScope,
                            destinationTableName,
                            query,
                            BatchSize,
                            GetPopulateBvInterviewQuotaCellTableCallback(quotaMatcher),
                            cancellationToken,
                            surveyConnectionInfo.SchemaName,
                            ReplicationSqlCommandTimeout,
                            options);
                }
                else
                {
                    recordsCopied = _remoteDataCopier.CopyDataToExistTable(
                        surveyConnectionInfo.ConnectionString,
                        connectionScope,
                        destinationTableName,
                        query,
                        surveyConnectionInfo.SchemaName,
                        ReplicationSqlCommandTimeout,
                        options);
                }
            }

            EventDetailsScope.Current.AddTiming("BulkInsertReplicatedDataToTable bulk insert", 500);

            return recordsCopied;
        }

        private bool IsPopulatingBvInterviewQuotaCellTableRequired(int surveyId, string destinationTableName)
        {
            return destinationTableName == _replicationSchemaInfoService.GetDestinationTableName(surveyId);
        }

        private Action<DataTable> GetPopulateBvInterviewQuotaCellTableCallback(QuotaMatcher quotaMatcher)
        {
            return (batch) => { _interviewQuotaCellService.PopulateBatch(quotaMatcher, batch); };
        }

        private void RunReplication(
            int surveyId,
            IExclusiveDatabaseLockFactory lockFactory,
            DatabaseTransactionOptions transactionOptions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ManagementActivityEvent<NoManagementParameters> evt;

            if (surveyId == 0)
            {
                evt = new PeriodicalReplicationEvent();
            }
            else
            {
                evt = new SurveyReplicationEvent();

                var survey = SurveyRepository.GetById(surveyId);
                evt.ObjectId = surveyId;
                evt.ObjectName = survey.Name;
            }

            var processedSurveys = 0;
            var disabledReplication = 0;
            var readSurveyVersionTimer = new Stopwatch();
            var obtainLockTimer = new Stopwatch();
            var updateClusterCellsTimer = new Stopwatch();
            var replicateTablesTimer = new Stopwatch();
            var validateVersionsTimer = new Stopwatch();
            var rereadTimer = new Stopwatch();

            using (new EventDetailsScope(evt.Details))
            {
                StringBuilder errorMessage = new StringBuilder();

                var surveys2Tables = GetGroupedReplicatedTables(surveyId);

                foreach (var surveyTables in surveys2Tables)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    ++processedSurveys;

                    surveyId = surveyTables.Key;

                    long surveyDatabaseVersion;

                    try
                    {

                        // Take lock per survey
                        obtainLockTimer.Start();
                        using (var dbLock = lockFactory.Create(DatabaseLockTimeoutsAndRecourceNames.GetSurveyReplicationResourceName(surveyId)))
                        {
                            try
                            {
                                if (!dbLock.TryEnterLock())
                                {
                                    continue;
                                }
                            }
                            finally
                            {
                                obtainLockTimer.Stop();
                            }

                            // Versions are tracked per database
                            readSurveyVersionTimer.Start();
                            surveyDatabaseVersion = GetSurveyDatabaseChangeTrackingVersion((int)surveyTables.First().SurveySid);
                            readSurveyVersionTimer.Stop();

                            ReplicationTableStatus replicationStatus = ReplicationTableStatus.None;

                            // Let's check replication status/versions and re read data if something is wrong
                            // Need to do it out of the replication cycle to avoid doing this several times.
                            validateVersionsTimer.Start();
                            var isReplicationVersionValid =
                                surveyTables.All(
                                    replicatedTable =>
                                        IsReplicationTableVersionValid(surveyDatabaseVersion, replicatedTable, surveyId));
                            validateVersionsTimer.Stop();

                            if (isReplicationVersionValid)
                            {
                                replicateTablesTimer.Start();
                                replicationStatus = ReplicateTables(
                                    transactionOptions,
                                    surveyTables,
                                    surveyDatabaseVersion,
                                    replicationStatus,
                                    errorMessage);
                                replicateTablesTimer.Stop();
                            }
                            else
                            {
                                rereadTimer.Start();
                                RereadSurveyReplicatedDataInternal(
                                    surveyId,
                                    surveyDatabaseVersion,
                                    string.Format("Versions are not valid, survey database version {0}",
                                        surveyDatabaseVersion), cancellationToken);

                                replicationStatus = ReplicationTableStatus.Reinitialize;
                                rereadTimer.Stop();
                            }

                            updateClusterCellsTimer.Start();
                            UpdateClusterCellIds(surveyTables.Key, replicationStatus, surveyTables.Select(s => s.TableName));
                            updateClusterCellsTimer.Stop();
                        } // using (var dbLock = lockFactory.Create(DatabaseLockTimeoutsAndRecourceNames.GetSurveyReplicationResourceName(surveyId)))
                    }
                    catch (DatabaseNotAvailableException)
                    {
                        ++disabledReplication;
                        UpdateSurveyReplicationStatus(surveyId, false, "Survey database is not available");
                    }
                    catch (ChangeTrackingNotEnabledException)
                    {
                        ++disabledReplication;
                        UpdateSurveyReplicationStatus(surveyId, false, "Change tracking is not enabled");
                    }
                    catch (SqlException ex) when (ex.Number == InvalidColumnNameSqlExceptionNumber)
                    {
                        ++disabledReplication;
                        UpdateSurveyReplicationStatus(surveyId, false, "Invalid replication schema");
                    }
                    catch (Exception ex)
                    {
                        errorMessage.AppendLine();
                        errorMessage.AppendFormat(
                            "Error occurred during replication. of Survey {0}\r\n\r\nException:\r\n{1}",
                            surveyId,
                            ex);
                    }
                } // foreach (var surveyTables in surveys2Tables)

                if (errorMessage.Length > 0)
                {
                    Trace.TraceError(errorMessage.ToString());
                }
            } // using (new EventDetailsScope(evt.Details))

            if (evt.Duration.TotalMilliseconds > 1000)
            {
                // To avoid too many messaged in the log write messages if replication took some time only
                LogReplicationMessage(evt.Details, "Surveys processed: ", processedSurveys);
                LogReplicationMessage(evt.Details, "Disabled replication: ", disabledReplication);
                LogReplicationMessage(evt.Details, "Aggregated read survey version: ", readSurveyVersionTimer.ElapsedMilliseconds);
                LogReplicationMessage(evt.Details, "Aggregated get survey lock: ", obtainLockTimer.ElapsedMilliseconds);
                LogReplicationMessage(evt.Details, "Aggregated validate versions: ", validateVersionsTimer.ElapsedMilliseconds);
                LogReplicationMessage(evt.Details, "Aggregated update cluster cells: ", updateClusterCellsTimer.ElapsedMilliseconds);
                LogReplicationMessage(evt.Details, "Aggregated replicate tables: ", replicateTablesTimer.ElapsedMilliseconds);
                LogReplicationMessage(evt.Details, "Aggregated reread data: ", rereadTimer.ElapsedMilliseconds);
            }

            evt.Finish();
        }

        private void LogReplicationMessage(IEventDetails eventDetails, string message, long counter)
        {
            if (counter > 0)
            {
                eventDetails.AddMessage(message + counter);
            }
        }

        private ReplicationTableStatus ReplicateTables(
            DatabaseTransactionOptions transactionOptions,
            IGrouping<int, BvSpGetReplicatedTableEntity> surveyTables,
            long surveyDatabaseVersion,
            ReplicationTableStatus replicationStatus,
            StringBuilder errorMessage)
        {

            DataTable mergedData = new DataTable();
            foreach (var replicatedTable in surveyTables)
            {
                try
                {
                    var (status, updatedData) = ReplicateTable(transactionOptions, replicatedTable, surveyDatabaseVersion);

                    if (status == ReplicationTableStatus.Update)
                        mergedData.Merge(updatedData, false, MissingSchemaAction.Add);

                    if (status > replicationStatus)
                    {
                        // Save MAX value in the replicationStatus variable so later it could be used to update cluster cell's
                        replicationStatus = status;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage.AppendLine();
                    errorMessage.AppendFormat(
                        "Error occurred during replication {0} table of {1} survey. ex: {2}",
                        replicatedTable.TableName,
                        replicatedTable.SurveySid,
                        ex);

                    if (StopReplicationException(ex))
                        throw;
                }
            }

            int surveyId = surveyTables.Key;
            if (mergedData.Rows.Count > 0)
                UpdateInterviewQuotaCells(surveyId, mergedData);

            return replicationStatus;
        }

        private bool StopReplicationException(Exception ex)
        {
            return ex is SqlException sqlException && sqlException.Number == InvalidColumnNameSqlExceptionNumber;
        }

        private void UpdateInterviewQuotaCells(int surveyId, DataTable replicatedInterviews)
        {
            //dont delete cells for deleted interviews as they will be deleted anyway by foreign key
            
            var updatedIds = replicatedInterviews.AsEnumerable()
                .Where(row => (string)row["SYS_CHANGE_OPERATION"] != "D")
                .Select(row => (int)row["respid"]).ToList();
            var batches = updatedIds.SplitIntoBatches(BatchSize);

            var quotaMatcher = _quotaMatcherBuilder.Build(surveyId);
            foreach (var ids in batches)
            {
                using (var transaction = new DatabaseTransactionScope(new DatabaseTransactionOptions($"{nameof(UpdateInterviewQuotaCells)}")))
                {
                    _interviewQuotaCellService.Delete(surveyId, ids);
                    var interviewsData = _replicatedDataRepository.GetInterviewsData(surveyId, ids);
                    _interviewQuotaCellService.PopulateBatch(quotaMatcher, interviewsData);
                    
                    transaction.Commit();
                }
            }
        }

        private bool IsReplicationTableVersionValid(
            long surveyDatabaseVersion,
            BvSpGetReplicatedTableEntity replicatedTable,
            int surveyId)
        {
            var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo((int)replicatedTable.SurveySid, updateLastConnectionTime: false);

            var minValidVersion =
                new DatabaseEngine(surveyConnectionInfo.ConnectionString).ExecuteScalarInNewConnection<object>(
            $"SELECT CHANGE_TRACKING_MIN_VALID_VERSION (OBJECT_ID('[{surveyConnectionInfo.SchemaName}].[{replicatedTable.TableName}]'))",
                    CommandType.Text) as long?;

            // TODO: Review/TBD should we actually throw here or it is better to enable change tracking? Need to solve it finally.

            // get the minimum version that is valid for use in obtaining change tracking information from the specified table
            if (minValidVersion == null)
            {
                var survey = _surveyRepository.GetById(surveyId);
                throw new ChangeTrackingNotEnabledException(
                    String.Format(
                        @"One of the following conditions is true:
                                                * Change tracking is not enabled for the project {0}.
                                                For the table {1} (id {2} in the BvReplicationTables table):
                                                * Table {1} does not exist.
                                                * Change tracking is not enabled for the table {1}.
                                                * The specified table {1} object ID is not valid for the current project {0}.
                                                * Insufficient permission to the table {1} specified by the object ID.",
                        survey.ProjectId,
                        replicatedTable.TableName,
                        replicatedTable.TableID));
            }

            if (replicatedTable.LastVersion == null ||
                // replication is called first time, WTF, it should never be called 

                // +1 is tp workaround the bug described in the following connect bugs
                //    https://connect.microsoft.com/SQLServer/feedback/details/770014/change-tracking-min-valid-version-returns-value-higher-than-change-tracking-current-version
                //    https://connect.microsoft.com/SQLServer/feedback/details/811766/change-tracking-min-valid-version-returns-value-higher-than-change-tracking-current-version
                //
                //    So, sometimes MinValidVersion is actually less than current version but it always less on just 1
                (replicatedTable.LastVersion.Value + 1) < minValidVersion ||
                // version in ReplicationData table isn't valid 
                //(looks like change tracking has cleared changes after retention period)
                replicatedTable.LastVersion.Value > surveyDatabaseVersion)
            {
                EventDetailsScope.Current.AddMessage(
                    "Local Version: {0} Survey Database Version {1} Minimum Valid Version {2}",
                    (replicatedTable.LastVersion == null ? "Null" : replicatedTable.LastVersion.Value.ToString()),
                    surveyDatabaseVersion,
                    minValidVersion);

                // version in ReplicatedData table isn't valid 
                //(looks like Cf has restored survey database from backup)
                return false;
            }

            return true;
        }

        private void UpdateSurveyReplicationStatus(int surveyId, bool status, string reason)
        {
            var survey = SurveyRepository.GetById(surveyId);

            survey.ReplicationStatus = status;
            BvSurveyAdapter.Update(survey);
            
            _sqlTableUpdatedPublisher.PublishSurveyUpdated();
            Trace.TraceError($"Replication for the project {survey.ProjectId} has been disabled. {reason}");
        }

        private void UpdateClusterCellIds(int surveyId, ReplicationTableStatus replicationStatus, IEnumerable<string> replicationTables)
        {
            var survey = _surveyRepository.GetById(surveyId);

            if (String.IsNullOrEmpty(survey.ClusteredQuotaName))
                return;

            switch (replicationStatus)
            {
                case ReplicationTableStatus.Reinitialize:
                    {
                        var query = String.Format("SELECT InterviewId as Id FROM BvSvySchedule WHERE SurveySID = {0}", surveyId);
                        _quotaClusteringSyncService.SyncCallsAndCounters(survey, new QueriedBatchParameters(query));
                    }
                    break;
                case ReplicationTableStatus.Update:
                    {
                        var query = GenerateIdQueryOfChangedInterviews(surveyId, replicationTables);
                        _quotaClusteringSyncService.SyncCallsAndCounters(survey, new QueriedBatchParameters(query));
                    }
                    break;
                case ReplicationTableStatus.None:
                    return;
                default:
                    throw new Exception("Unknown replication table status");
            }
        }

        private string GenerateIdQueryOfChangedInterviews(int surveyId, IEnumerable<string> replicatedTableNames)
        {
            var transferTableNames = replicatedTableNames.Select(replName => ReplicationSchemaService.GetTransferChangesTableName(replName, surveyId));
            return StringService.Join(" UNION ALL ", "SELECT respid as Id FROM {0}", transferTableNames);
        }

        private IEnumerable<IGrouping<int, BvSpGetReplicatedTableEntity>> GetGroupedReplicatedTables(int surveyId)
        {
            var replicatedTables = BvSpGetReplicatedTableAdapter.ExecuteEntityList();

            IEnumerable<IGrouping<int, BvSpGetReplicatedTableEntity>> surveys2tables;
            if (surveyId != 0)
            {
                surveys2tables = replicatedTables.Where(x => x.SurveySid == surveyId).GroupBy(y => (int)y.SurveySid);
            }
            else
            {
                var surveys = replicatedTables.Select(x => x.ProjectId);
                var activeSurveys = _projectsActivityService.GetActiveProjectIds(surveys).ToHashSet();
                surveys2tables = replicatedTables.Where(x => activeSurveys.Contains(x.ProjectId)).GroupBy(y => (int)y.SurveySid);
            }
            return surveys2tables;
        }

        private (ReplicationTableStatus, DataTable) ReplicateTable(
            DatabaseTransactionOptions transactionOptions,
            BvSpGetReplicatedTableEntity replicatedTable,
            long surveyDatabaseVersion)
        {
            // We have up to date version, so we do not need to replicate anything.
            if (replicatedTable.LastVersion != null && replicatedTable.LastVersion.Value == surveyDatabaseVersion)
            {
                return (ReplicationTableStatus.None, null);
            }

            DataTable updatedData;
            using (var transactionScope = new DatabaseTransactionScope(transactionOptions))
            {
                if (replicatedTable.TableName.StartsWith("response", StringComparison.OrdinalIgnoreCase))
                {
                    updatedData = UpdateReplicatedDataForResponseTable(
                                            replicatedTable.LastVersion.Value,
                                            surveyDatabaseVersion,
                                            replicatedTable.DestinationTableName,
                                            replicatedTable.TableName,
                                            replicatedTable.TableID.Value,
                                            replicatedTable.SurveySid.Value);
                }
                else if (replicatedTable.TableName.CompareTo("respondent") == 0)
                {
                    updatedData = UpdateReplicatedDataForRespondentTable(
                        replicatedTable.LastVersion.Value,
                        surveyDatabaseVersion,
                        replicatedTable.DestinationTableName,
                        replicatedTable.TableID.Value,
                        replicatedTable.SurveySid.Value);
                }
                else
                {
                    throw new Exception(string.Format("Unknown replication table {0}", replicatedTable.TableName));
                }

                transactionScope.Commit();
            }

            // update last version in the BvReplicationTables table
            new DatabaseEngine().ExecuteNonQuery(
                String.Format(
                    @"update BvReplicationTables set LastVersion = {1} where ID = {0}", replicatedTable.TableID, surveyDatabaseVersion),
                CommandType.Text);

            return (ReplicationTableStatus.Update, updatedData);
        }

        private long GetSurveyDatabaseChangeTrackingVersion(int surveySid)
        {
            try
            {
                var version = new DatabaseEngine(_surveyConnectionStringProvider.GetConnectionInfo(surveySid, false).ConnectionString).ExecuteScalarInNewConnection<object>(
                    "SELECT CHANGE_TRACKING_CURRENT_VERSION()",
                    CommandType.Text) as long?;

                if (version == null)
                {
                    throw new InvalidOperationException(
                        String.Format("Change tracking is not enabled for survey {0}.", surveySid));
                }

                return (long)version;
            }
            catch (SqlException e)
            {
                if (IsDatabaseNotAvailableError(e))
                {
                    throw new DatabaseNotAvailableException("for survey " + surveySid, e);
                }

                throw;
            }

        }

        public static bool IsDatabaseNotAvailableError(SqlException e)
        {
            return e.Number == 911 || e.Number == 4060;
        }

        /// <summary>
        /// Returns the string for columns update.
        /// </summary>
        /// <returns></returns>
        private static string GetColumnUpdateString(IEnumerable<string> columnNames, string targetMultipartIdentifier)
        {
            return String.Join(",", columnNames.Select(column => String.Format("{0}{1} = Source.{1}", targetMultipartIdentifier, column)).ToArray());
        }

        ///  <summary>
        ///  Replicates response table data
        ///  </summary>
        ///  <param name="lastVersion">Latest local version</param>
        ///  <param name="remoteVersion">Survey database version</param>
        ///  <param name="destinationTableName">Destination table for replication</param>
        /// <param name="sourceTableName"></param>
        /// <param name="tableID">Table ID</param>
        ///  <param name="surveySid">Survey to replicate</param>
        private DataTable UpdateReplicatedDataForResponseTable(
            long lastVersion,
            long remoteVersion,
            string destinationTableName,
            string sourceTableName,
            int tableID,
            int surveySid)
        {
            var survey = SurveyRepository.GetById(surveySid);

            var replicatedColumns = ReplicationColumnsRepository.GetByTableId(tableID);
            string[] columns = replicatedColumns.Select(x => "[" + x.ColumnName + "]").ToArray();

            string clauseForTrackedColumns = GeneratedClauseForTrackedColumns(replicatedColumns.Select(x => x.ColumnID));

            string transferTableName = ReplicationSchemaService.GetTransferChangesTableName(sourceTableName, surveySid);

            //
            // we ignore cases when deletion is occurred from response table therefore this records should be deleted from respondent as well
            //
            var columnsForSelect = string.Join(",", columns);
            string copyDataQuery = $@"
                SELECT ct.SYS_CHANGE_OPERATION, ct.SYS_CHANGE_COLUMNS, r.respid, {columnsForSelect}
                FROM CHANGETABLE(CHANGES <Schema>.{sourceTableName}, {lastVersion}) ct
                LEFT JOIN <Schema>.{sourceTableName} r ON r.responseid = ct.responseid
                WHERE r.respid IS NOT NULL AND ct.SYS_CHANGE_VERSION <= {remoteVersion}";

            var databaseEngine = new DatabaseEngine();
            int changesProcessed;
            string query;
            using (var connectionScope = new ConnectionScope())
            {
                query = $"TRUNCATE TABLE {transferTableName}";
                databaseEngine.ExecuteNonQuery(query);

                var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveySid, updateLastConnectionTime: false);
                changesProcessed = _remoteDataCopier.CopyDataToExistTable(
                    surveyConnectionInfo.ConnectionString,
                    connectionScope,
                    transferTableName,
                    copyDataQuery,
                    surveyConnectionInfo.SchemaName,
                    ReplicationSqlCommandTimeout);

                if (changesProcessed > 0)
                {
                    query = $@"
                        UPDATE {destinationTableName}
                        SET    {GetColumnUpdateString(columns, "")} 
                        FROM   {destinationTableName} Target
                        INNER JOIN 
                            (SELECT respid, {columnsForSelect}, SYS_CHANGE_OPERATION, SYS_CHANGE_COLUMNS FROM {transferTableName}) Source
                        ON
                            Target.respid = Source.respid AND ({clauseForTrackedColumns})";

                    databaseEngine.ExecuteNonQueryWithSpecificTimeOut(query, CommandType.Text, ReplicationSqlCommandTimeout);
                }
            }

            var selectQuery = $"SELECT respid, SYS_CHANGE_OPERATION  FROM {transferTableName}";
            var updatedData = databaseEngine.ExecuteDataTable<DataTable>(selectQuery, CommandType.Text);

            if (changesProcessed > 0)
            {
                var message = string.Format(
                    "Replicated {0} table for survey {1} '{2}'. Processed {3} INSERT/DELETE/UPDATE changes from version {4} to version {5}",
                    sourceTableName,
                    survey.Name,
                    survey.Description,
                    changesProcessed,
                    lastVersion,
                    remoteVersion);
                EventDetailsScope.Current.AddMessage(message);
            }

            EventDetailsScope.Current.AddTiming(string.Format("Query\r\n{0}", copyDataQuery + "\r\n" + query), 1000);

            return updatedData;
        }

        private DataTable UpdateReplicatedDataForRespondentTable(
            long lastVersion,
            long remoteVersion,
            string destinationTableName,
            int tableID,
            int surveySid)
        {
            var sourceTableName = "respondent";

            var survey = SurveyRepository.GetById(surveySid);

            var replicatedColumns = ReplicationColumnsRepository.GetByTableId(tableID);
            string[] columns = replicatedColumns.Select(x => "[" + x.ColumnName + "]").ToArray();

            string transferTableName = ReplicationSchemaService.GetTransferChangesTableName(sourceTableName, surveySid);

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // For the respondent table we're inserting data while sample addition, so we ignore insert in the replication aka "I" changes
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            string clauseForTrackedColumns = GeneratedClauseForTrackedColumns(replicatedColumns.Select(x => x.ColumnID));

            //
            // we ignore cases when deletion is occurred from response table therefore this records should be deleted from respondent as well
            //
            var columnsForSelect = string.Join(",", columns);
            string copyDataQuery = $@"
                SELECT ct.SYS_CHANGE_OPERATION, ct.SYS_CHANGE_COLUMNS, ct.respid, {columnsForSelect}
                FROM CHANGETABLE(CHANGES <Schema>.{sourceTableName}, {lastVersion}) ct
                LEFT JOIN <Schema>.{sourceTableName} r ON r.respid = ct.respid
                WHERE ct.respid IS NOT NULL AND (NOT(ct.SYS_CHANGE_OPERATION = 'I' AND ct.SYS_CHANGE_VERSION = ct.SYS_CHANGE_CREATION_VERSION)) AND ct.SYS_CHANGE_VERSION <= {remoteVersion}";

            var databaseEngine = new DatabaseEngine();
            int changesProcessed;
            string query;
            using (var connectionScope = new ConnectionScope())
            {
                query = $"TRUNCATE TABLE {transferTableName}";
                databaseEngine.ExecuteNonQuery(query);

                var surveyConnectionInfo = _surveyConnectionStringProvider.GetConnectionInfo(surveySid, updateLastConnectionTime: false);
                changesProcessed = _remoteDataCopier.CopyDataToExistTable(
                    surveyConnectionInfo.ConnectionString,
                    connectionScope,
                    transferTableName,
                    copyDataQuery,
                    surveyConnectionInfo.SchemaName,
                    ReplicationSqlCommandTimeout);

                if (changesProcessed > 0)
                {
                    query = $@"
                        MERGE INTO {destinationTableName} AS Target
                        USING ( SELECT respid, {columnsForSelect}, SYS_CHANGE_OPERATION, SYS_CHANGE_COLUMNS
                            FROM {transferTableName} ct) 
                        AS Source (respid, {columnsForSelect}, SYS_CHANGE_OPERATION, SYS_CHANGE_COLUMNS)
                        ON Target.respid = Source.respid
                        WHEN MATCHED AND ((Source.SYS_CHANGE_OPERATION = 'U' OR Source.SYS_CHANGE_OPERATION = 'I') AND ({clauseForTrackedColumns})) THEN
                        UPDATE SET {GetColumnUpdateString(columns, "Target.")}
                        WHEN MATCHED AND Source.SYS_CHANGE_OPERATION = 'D' THEN DELETE;

                        SELECT ct.respid
                        FROM {transferTableName} ct
                        WHERE ct.SYS_CHANGE_OPERATION = 'D'";

                    var deletedIds = databaseEngine.ExecuteScalarListWithSpecificTimeOut<int>(query, CommandType.Text, ReplicationSqlCommandTimeout);

                    if (deletedIds.Count > 0)
                    {
                        var title = $"Delete Respondents from survey '{survey.Name}' ({survey.Description})";

                        var asyncOperationQueue = ServiceLocator.Resolve<IAsyncOperationQueue>();
                        var param = new AsyncOperations.Operations.DeleteRespondents.Parameters
                        {
                            SurveyId = survey.SID,
                            ProjectId = survey.ProjectId,
                            RespondentIds = deletedIds.ToArray()
                        };

                        asyncOperationQueue.Enqueue(
                            0,
                            title,
                            true,
                            param,
                            AsyncOperationConstants.HighPriority,
                            null);
                    }
                }
            }

            var selectQuery = $"SELECT respid, SYS_CHANGE_OPERATION FROM {transferTableName}";
            var updatedData = databaseEngine.ExecuteDataTable<DataTable>(selectQuery, CommandType.Text);


            if (changesProcessed > 0)
            {
                var message = string.Format(
                    "Replicated respondent table for survey {0} '{1}'. Processed {2} UPDATE/DELETE changes from version {3} to version {4}",
                    survey.Name,
                    survey.Description,
                    changesProcessed,
                    lastVersion,
                    remoteVersion);
                EventDetailsScope.Current.AddMessage(message);
            }

            EventDetailsScope.Current.AddTiming(string.Format("Replication Query\r\n{0}", query), 1000);

            return updatedData;
        }

        /// <summary>
        /// Generates the string for update clause.
        /// </summary>
        /// <param name="ids">Columns ids</param>
        /// <returns></returns>
        private static string GeneratedClauseForTrackedColumns(IEnumerable<int> ids)
        {
            return String.Join(" OR ", ids.Select(id => String.Format("CHANGE_TRACKING_IS_COLUMN_IN_MASK( {0}, Source.SYS_CHANGE_COLUMNS ) = 1", id)).ToArray());
        }

        /// <summary>
        /// Gets the value of the specified variable for the specified interview from the replicated data.
        /// </summary>
        /// <param name="surveySid">The survey SID.</param>
        /// <param name="interviewID">The ID of the interview to get data for.</param>
        /// <param name="columnName">Name of the variable.</param>
        /// <returns>The value of the variable.</returns>
        internal static string GetReplicationValue(int surveySid, int interviewID, string columnName)
        {
            using (var connection = new SqlConnection(BackendInstance.Current.ConnectionString))
            {
                var query = String.Format("SELECT CAST( [{0}] AS NVARCHAR(MAX)) FROM {1} WHERE {2} = @IID",
                    columnName,
                    ReplicationSchemaService.GetDestinationTableName(surveySid),
                    ReplicationTablePrimaryKey);

                var command = new SqlCommand(query, connection);
                command.Parameters.Add(new SqlParameter("@IID", interviewID));

                connection.Open();

                return command.ExecuteScalar() as string;
            }
        }

        public int GetNumberOfReplicationRecords(string projectId, int respid)
        {
            var survey = _surveyRepository.GetByName(projectId);
            var destinationTableName = _replicationSchemaInfoService.GetDestinationTableName(survey.SID);

            var databaseEngine = new DatabaseEngine();

            var replicationRecords = databaseEngine.ExecuteScalar<int>(
                string.Format(
                    "SELECT COUNT(*) FROM {0} WHERE {0}.respid = {1}", destinationTableName,
                    respid),
                    CommandType.Text);

            return replicationRecords;
        }

        public void ReplicateInterviewData(BvSurveyEntity survey, int respondentId)
        {
            var numberOfReplicationRecords = GetNumberOfReplicationRecords(survey.ProjectId, respondentId);

            if (numberOfReplicationRecords == 0)
            {
                var tableName = _replicationSchemaInfoService.GetDestinationTableName(survey.SID);
                CopyReplicationDataToTable(survey.SID, tableName, 0, respondentId, CancellationToken.None);
            }
        }
    }
}