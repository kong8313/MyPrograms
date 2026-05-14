using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations.ConfigureClusteredQuota;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.Batch.Tools;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Framework.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.Core.Services
{
    public class QuotaClusteringService : IQuotaClusteringConfigurationService, IQuotaClusteringSyncService
    {
        private readonly Lazy<ISurveyRepository> _surveyRepository;
        private readonly Lazy<IAsyncOperationQueue> _asyncOperationQueue;
        private readonly Lazy<IQuotaInfoService> _quotaInfoService;
        private readonly Lazy<IReplicationSchemaInfoService> _replicationSchemaInfoService;
        private readonly Lazy<IRetryingService> _retryingService;
        private readonly Lazy<ISupervisorNameProvider> _supervisorNameProvider;
        private readonly Lazy<IBatchFactory> _batchFactory;
        private readonly Lazy<IDatabaseBatchItemTransfer> _databaseBatchItemTransfer;
        private readonly Lazy<ISurveyConnectionStringProvider> _surveyConnectionStringProvider;
        private readonly Lazy<IRemoteDataCopier> _remoteDataCopier;

        private readonly int BatchSize = 1000;

        public QuotaClusteringService()
        {
            _surveyRepository = new Lazy<ISurveyRepository>(() => ServiceLocator.Resolve<ISurveyRepository>());
            _asyncOperationQueue = new Lazy<IAsyncOperationQueue>(() => ServiceLocator.Resolve<IAsyncOperationQueue>());
            _quotaInfoService = new Lazy<IQuotaInfoService>(() => ServiceLocator.Resolve<IQuotaInfoService>());
            _replicationSchemaInfoService = new Lazy<IReplicationSchemaInfoService>(() => ServiceLocator.Resolve<IReplicationSchemaInfoService>());
            _retryingService = new Lazy<IRetryingService>(() => ServiceLocator.Resolve<IRetryingService>());
            _supervisorNameProvider = new Lazy<ISupervisorNameProvider>(() => ServiceLocator.Resolve<ISupervisorNameProvider>());
            _batchFactory = new Lazy<IBatchFactory>(() => ServiceLocator.Resolve<IBatchFactory>());
            _databaseBatchItemTransfer = new Lazy<IDatabaseBatchItemTransfer>(() => ServiceLocator.Resolve<IDatabaseBatchItemTransfer>());
            _surveyConnectionStringProvider = new Lazy<ISurveyConnectionStringProvider>(() => ServiceLocator.Resolve<ISurveyConnectionStringProvider>());
            _remoteDataCopier = new Lazy<IRemoteDataCopier>(() => ServiceLocator.Resolve<IRemoteDataCopier>());
        }

        public QuotaClusteringConfiguration GetConfiguration(int surveyId)
        {
            var survey = _surveyRepository.Value.GetById(surveyId);

            return new QuotaClusteringConfiguration
            {
                QuotaName = survey.ClusteredQuotaName,
                LiveThreshod = survey.ClusteredQuotaThreshold
            };
        }

        public void Configure(int surveyId, QuotaClusteringConfiguration configuration)
        {
            var survey = _surveyRepository.Value.GetById(surveyId);

            survey.ClusteredQuotaName = configuration.QuotaName;
            survey.ClusteredQuotaThreshold = configuration.LiveThreshod;

            _surveyRepository.Value.Update(survey);

            var parameters = new Parameters()
            {
                SurveyId = surveyId,
                LiveThreshold = configuration.LiveThreshod,
                QuotaName = configuration.QuotaName
            };

            string title = String.Format("Configure clustered quota for survey \"{0}\"({1})", survey.Name,
                survey.Description);

            _asyncOperationQueue.Value.Enqueue(
                0,
                title,
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                _supervisorNameProvider.Value.Name);
        }

        public void Reset(int surveyId)
        {
            var survey = _surveyRepository.Value.GetById(surveyId);

            survey.ClusteredQuotaName = null;
            survey.ClusteredQuotaThreshold = 0;

            _surveyRepository.Value.Update(survey);

            var parameters = new Parameters()
            {
                SurveyId = surveyId,
                LiveThreshold = 0,
                QuotaName = null
            };

            string title = String.Format("Reset clustered quota for survey \"{0}\"({1})", survey.Name,
                survey.Description);

            _asyncOperationQueue.Value.Enqueue(
                0,
                title,
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                _supervisorNameProvider.Value.Name);
        }

        public bool IsEnabled(int surveyId)
        {
            var survey = _surveyRepository.Value.GetById(surveyId);
            return !String.IsNullOrEmpty(survey.ClusteredQuotaName);
        }

        public bool IsEnabled(BvSurveyEntity survey)
        {
            return !String.IsNullOrEmpty(survey.ClusteredQuotaName);
        }

        public void ResetCallsAndCounters(BvSurveyEntity survey, CancellationToken cancellationToken)
        {
            var liveCounterQuery = "DELETE FROM [BvClusteredQuotaCell] WHERE SurveyId = @SurveyId";

            var processBatchQuery = String.Format(
                @"
                DECLARE @LastIID INT = -1
                ;WITH data as
	            (
		            SELECT TOP({0}) c.*
		            FROM BvSvySchedule c
			            WHERE c.SurveySID = @SurveyId AND c.InterviewID > @PreviosIID
			            ORDER BY c.SurveySID, c.InterviewID
	            )
	            UPDATE data
		            SET CellId = 0,
			            @LastIID = CASE WHEN @LastIID > InterviewId THEN @LastIID ELSE InterviewId END 
		            
                SELECT @LastIID",
                BatchSize);

            var db = new DatabaseEngine();

            db.ExecuteNonQuery(
                    liveCounterQuery,
                    CommandType.Text,
                    new SqlParameter("@SurveyId", survey.SID));

            var prevIID = 0;
            while (prevIID >= 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                _retryingService.Value.Retry(3,
                    String.Format("Update cell id for interview batch(PreviosIID = {0})", prevIID),
                    () =>
                    {
                        prevIID = db.ExecuteScalar<int>(
                            processBatchQuery,
                            CommandType.Text,
                            new SqlParameter("@SurveyId", survey.SID),
                            new SqlParameter("@PreviosIID", prevIID));
                    });
            }
        }

        public void SyncCallsAndCounters(BvSurveyEntity survey, BatchParameters batchParameters)
        {
            if (!IsEnabled(survey))
                return;

            Bv_ClusterQuotaService_GetCellIdQueryAdapter.ExecuteNonQuery(survey.SID, survey.ClusteredQuotaName, "repl", out var idQuery);

            var processBatchQuery = String.Format(
                @"
                CREATE TABLE #changedCells( NewCellId INT , OldCellId INT, CallState INT )
                ;WITH data as
	            (
		            SELECT c.*, ({1}) as newCellId
                    FROM BvSvySchedule c
		            INNER JOIN BvTransferArrays iids ON c.InterviewId = iids.ItemId
                    LEFT JOIN {0} repl ON c.InterviewID = repl.respid
		            WHERE c.SurveySID = @SurveyId AND iids.BatchId = @BatchId
	            )
	            UPDATE data
		            SET CellId = ISNULL( newCellId, 0 )
                    OUTPUT inserted.CellId, deleted.CellId, inserted.CallState INTO #changedCells
                
                ;WITH changedCells AS
                (
                    SELECT CellId, SUM(Cnt) as DiffCnt FROM 
                    (
                        SELECT NewCellId as CellId, COUNT(*) as Cnt FROM #changedCells WHERE CallState IN ( -2, -1 ) GROUP BY NewCellId
                        UNION 
                        SELECT OldCellId as CellId, -COUNT(*) as Cnt FROM #changedCells WHERE CallState IN ( -2, -1 ) GROUP BY OldCellId
                    ) t 
                    GROUP BY CellId
                    HAVING SUM(Cnt) <> 0
                )
                UPDATE BvClusteredQuotaCell
                    SET LiveCount = LiveCount + DiffCnt
                    FROM changedCells
                    WHERE BvClusteredQuotaCell.SurveyId = @SurveyId AND BvClusteredQuotaCell.CellId = changedCells.CellId

                ",
                _replicationSchemaInfoService.Value.GetDestinationTableName(survey.SID),
               idQuery);

            using (var batch = _batchFactory.Value.CreateDatabaseBatch(batchParameters))
            using (var subBatch = _batchFactory.Value.CreateEmptyDatabaseBatch())
            {
                while (_databaseBatchItemTransfer.Value.TransferTo(batch, subBatch, 10000))
                {
                    using (var transaction = new DatabaseTransactionScope("ResyncClusteredCallsAndCounters"))
                    {
                        new DatabaseEngine().ExecuteNonQuery(processBatchQuery,
                            CommandType.Text,
                            new SqlParameter("@SurveyId", survey.SID),
                            new SqlParameter("@BatchId", subBatch.Id));

                        transaction.Commit();
                    }

                    subBatch.Clear();
                }
            }
        }

        public ReinitializeQuotaClusteringStatus ReinitializeCallsAndCounters(BvSurveyEntity survey,
            Action<string> taskLog, CancellationToken cancellationToken)
        {
            if (!IsEnabled(survey))
            {
                return ReinitializeQuotaClusteringStatus.NotChanged;
            }

            taskLog("Applying cluster quota settings...");

            if (!_quotaInfoService.Value.IsExists(survey, survey.ClusteredQuotaName))
            {
                survey.ClusteredQuotaName = null;

                _surveyRepository.Value.Update(survey);

                ResetCallsAndCounters(survey, cancellationToken);

                return ReinitializeQuotaClusteringStatus.Disabled;
            }

            if (IsQuotaStuctureChanged(survey))
            {
                InitializeCallsAndCounters(survey, cancellationToken);
                return ReinitializeQuotaClusteringStatus.Changed;
            }

            return ReinitializeQuotaClusteringStatus.NotChanged;
        }

        public bool IsQuotaStuctureChanged(BvSurveyEntity survey)
        {
            var fields = _quotaInfoService.Value.GetQuotaFields(survey.SID, survey.ClusteredQuotaName);

            using (var connectionScope = new ConnectionScope())
            {
                CreateTableWithCellsInfo(survey, fields, "#quotacells", connectionScope);

                var diffCount = new DatabaseEngine().ExecuteScalar<int>(
                   @"with cur as 
                    (
	                    select cellId, Name from BvClusteredQuotaCell c where SurveyId = @SurveyId
                    )
                    select COUNT(*) from cur
                    full join #quotacells new on cur.CellId = new.cellId
                    WHERE cur.CellId IS NULL OR new.CellId IS NULL OR cur.Name <> new.Name
                    ",
                     CommandType.Text, new SqlParameter("@SurveyId", survey.SID));

                return diffCount != 0;
            }
        }

        public void InitializeCallsAndCounters(BvSurveyEntity survey, CancellationToken cancellationToken)
        {
            var fields = _quotaInfoService.Value.GetQuotaFields(survey.SID, survey.ClusteredQuotaName);

            var processBatchQuery = String.Format(
                @"
                DECLARE @LastIID INT = -1
                ;WITH data as
	            (
		            SELECT TOP({0}) c.*, qc.cellId as newCellId
		            FROM BvSvySchedule c
                    LEFT JOIN {1} r 
			            ON c.InterviewID = r.respid
		            LEFT JOIN #quotacells qc 
			            ON {2}
			            WHERE c.SurveySID = @SurveyId AND c.InterviewID > @PreviosIID
			            ORDER BY c.SurveySID, c.InterviewID
	            )
	            UPDATE data
		            SET CellId = ISNULL( newCellId, 0 ),
			            @LastIID = CASE WHEN @LastIID > InterviewId THEN @LastIID ELSE InterviewId END 
		            
                SELECT @LastIID",
                BatchSize,
                _replicationSchemaInfoService.Value.GetDestinationTableName(survey.SID),
                StringService.Join(" AND ", "r.[{0}] = qc.[{0}]", fields));

            var liveCounterQuery = String.Format(
                @"DELETE FROM [BvClusteredQuotaCell] WHERE SurveyId = @SurveyId
                INSERT INTO BvClusteredQuotaCell(SurveyId, CellId, Name, LiveCount, LiveLimit) 
                    SELECT @SurveyId, q.cellid, q.Name, ISNULL(c.Cnt, 0), {0} FROM #quotacells q
                        LEFT JOIN ( SELECT CellId, COUNT(*) as Cnt FROM BvSvySchedule WHERE SurveySID = @SurveyId AND CallState IN ( -2, -1 ) GROUP BY CellId) c 
                        ON q.CellId = c.CellId",
                        survey.ClusteredQuotaThreshold);

            var cleanupQuery = "DROP TABLE #quotacells";

            using (var connectionScope = new ConnectionScope())
            {
                var db = new DatabaseEngine();

                cancellationToken.ThrowIfCancellationRequested();
                CreateTableWithCellsInfo(survey, fields, "#quotacells", connectionScope);

                try
                {
                    int? prevIID = 0;

                    while (prevIID >= 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        _retryingService.Value.Retry(3,
                            String.Format("Update cell id for interview batch(PreviosIID = {0})", prevIID),
                            () =>
                            {
                                prevIID = db.ExecuteScalar<int>(
                                    processBatchQuery,
                                    CommandType.Text,
                                    new SqlParameter("@SurveyId", survey.SID),
                                    new SqlParameter("@PreviosIID", prevIID));
                            });
                    }
                }
                finally
                {
                    db.ExecuteNonQuery(
                            liveCounterQuery,
                            CommandType.Text,
                            new SqlParameter("@SurveyId", survey.SID));

                    db.ExecuteNonQuery(
                            cleanupQuery,
                            CommandType.Text);
                }
            }
        }

        private void CreateTableWithCellsInfo(BvSurveyEntity survey, string[] fields, string tableName, IConnectionProvider connectionProvider)
        {
            var quotaTableName = _quotaInfoService.Value.GetQuotaTable(survey, survey.ClusteredQuotaName);
            var surveyConnectionInfo = _surveyConnectionStringProvider.Value.GetConnectionInfo(survey.SID);

            const string tableAlias = "q";
            Bv_ClusterQuotaService_GetCellNameQueryAdapter.ExecuteNonQuery(survey.SID, survey.ClusteredQuotaName, tableAlias, out var nameQuery);
           
            var fieldsQuery = StringService.Join(",", "ISNULL([{0}],'') as {0}", fields);
            var copyDataQuery = $"SELECT quotaid as cellId, {nameQuery} as Name, {fieldsQuery} FROM <Schema>.[{quotaTableName}] {tableAlias}";

            _remoteDataCopier.Value.CopyDataToNewTable(surveyConnectionInfo.ConnectionString, connectionProvider, tableName, copyDataQuery, surveyConnectionInfo.SchemaName);
        }
    }
}
