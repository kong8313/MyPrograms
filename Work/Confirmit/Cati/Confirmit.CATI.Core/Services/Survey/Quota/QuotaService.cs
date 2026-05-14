using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Logging;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.ManagementService;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.DataBaseLockServiceImplementation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.WcfServices.Clients;
using ConfirmitDialerInterface;
using Confirmit.CATI.Core.Services.Survey.Quota;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;

namespace Confirmit.CATI.Core.Services
{
    public class QuotaService : IQuotaInfoService, IFcdQuotaService
    {
        private readonly ISystemSettings _systemSettings;
        private readonly ISupervisorNameProvider _supervisorNameProvider;
        private readonly IDatabaseLockTimeouts _databaseLockTimeouts;
        private readonly IAuthoringService _authoringService;
        private readonly ISeQuotaRepository _seQuotaRepository;
        private readonly ISeQuotaCellRepository _seQuotaCellRepository;
        private readonly IQuotaRepository _quotaRepository;
        private readonly IQuotaCellRepository _quotaCellRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IQuotaDatabaseReader _quotaDatabaseReader;
        private readonly IInterviewQuotaCellService _interviewQuotaCellService;
        private readonly QuotaCellsUpdater _quotaCellUpdater;
        public QuotaService(
            ISystemSettings systemSettings,
            ISupervisorNameProvider supervisorNameProvider,
            IDatabaseLockTimeouts databaseLockTimeouts,
            IAuthoringService authoringService,
            ISeQuotaRepository seQuotaRepository,
            ISeQuotaCellRepository seQuotaCellRepository,
            IQuotaRepository quotaRepository,
            IQuotaCellRepository quotaCellRepository,
            ISurveyRepository surveyRepository,
            IQuotaDatabaseReader quotaDatabaseReader,
            IInterviewQuotaCellService interviewQuotaCellService,
            QuotaCellsUpdater quotaCellsUpdater
            )
        {
            _systemSettings = systemSettings;
            _supervisorNameProvider = supervisorNameProvider;
            _databaseLockTimeouts = databaseLockTimeouts;
            _authoringService = authoringService;
            _seQuotaRepository = seQuotaRepository;
            _seQuotaCellRepository = seQuotaCellRepository;
            _quotaRepository = quotaRepository;
            _quotaCellRepository = quotaCellRepository;
            _surveyRepository = surveyRepository;
            _quotaDatabaseReader = quotaDatabaseReader;
            _interviewQuotaCellService = interviewQuotaCellService;
            _quotaCellUpdater = quotaCellsUpdater;
        }

        public void OnQuotaCellChanged(int surveyId, int quotaId, int cellId, QuotaCellState state)
        {
            var isOpen = state == QuotaCellState.PessimisticallyOpened;

            List<int> openedCellsIds;
            List<int> closedCellsIds;
            using (var dbLock = ExclusiveDatabaseLock.CreateLock(DatabaseLockTimeoutsAndRecourceNames.GetFcdResourceName(surveyId), "OnQuotaCellChanged", _databaseLockTimeouts.DefaultLockTimeoutInMs))
            {
                dbLock.EnterLock();

                var result = ImportSurveyQuotaCell(surveyId, quotaId, cellId);
                openedCellsIds = result.OpenedAnyCellsIds;
                closedCellsIds = result.ClosedAnyCellsIds;
            }

            var quota = _quotaRepository.TryGetById(surveyId, quotaId);
            var cell = _quotaCellRepository.TryGetById(surveyId, quotaId, cellId);

            if (isOpen)
            {
                openedCellsIds.Add(cellId);
                OpenCell(surveyId, quotaId, openedCellsIds, quota, cell);
            }

            if (state == QuotaCellState.OptimisticallyClosed || state == QuotaCellState.PessimisticallyClosed)
            {
                closedCellsIds.Add(cellId);
                CloseCell(surveyId, quotaId, closedCellsIds, quota, cell);
            }
        }

        private void UpdateAnyCells(int surveyId)
        {
            using (var transactionScope = new DatabaseTransactionScope("UpdateQuotaCells"))
            {
                var quotas = _quotaDatabaseReader.GetQuotas(surveyId).ToList();

                foreach (var quotaInfo in quotas)
                {
                    _quotaCellUpdater.UpdateQuotaCells(surveyId, quotaInfo.Id);
                }

                transactionScope.Commit();
            }
        }

        public static string GetIterviewIdQueryForCell(int surveyId, string[] quotaFields, string[] cellValues)
        {
            return String.Format(
                    @"  SELECT r.respId as Id
                    FROM BvReplicatedData_{0} r
                    WHERE {1}",
                    surveyId,
                    GetCellWhereForRepicationTable("r", quotaFields, cellValues));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="quotaId"></param>
        /// <param name="cellId">If whole quota is updated then cfCellId = 0</param>
        /// <param name="quota"></param>
        /// <param name="cell"></param>
        private void CloseCell(int surveyId, int quotaId, List<int> cellsIds, BvSurveyQuotaEntity quota, BvSurveyQuotaCellEntity cell)
        {
            var batchParameters = new FilteredByClosedQuotaCellBatchParameters(surveyId, quotaId, cellsIds);

            IAsyncOperationParameters parameters;
            string title;

            switch (_systemSettings.FCD.AlgorithmType)
            {
                case FcdAlgorithmType.DeleteCalls:
                    parameters = new AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters
                    {
                        SurveyId = surveyId,
                        NewITS = (int)CallOutcome.FilteredByCallDelivery,
                        BatchParameters = batchParameters
                    };

                    if (quota != null && cell != null)
                    {
                        title = $"Delete calls for closed quota \"{quota.Name}\" cell \"{GetCellInformation(cell)}\"";
                    }
                    else
                    {
                        title = "Delete calls for closed quota cell";
                    }

                    break;

                case FcdAlgorithmType.DisableCallsWithReenabling:
                    parameters = new AsyncOperations.Operations.CallsManagementOperations.EnableCalls.Parameters
                    {
                        SurveyId = surveyId,
                        EnablingState = false,
                        BatchParameters = batchParameters,
                        IsFcdOperation = true
                    };

                    if (quota != null && cell != null)
                    {
                        title = $"Disable calls for closed quota \"{quota.Name}\" cell \"{GetCellInformation(cell)}\"";
                    }
                    else
                    {
                        title = "Disable calls for closed quota cell";
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }

            ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                0,
                title,
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                _supervisorNameProvider.Name);
        }

        private static string GetCellInformation(BvSurveyQuotaCellEntity cell)
        {
            var fields = cell.Data.FieldValues.Select(x => $"{x.Field}={x.Value}").JoinInString(", ");
            return fields;
        }

        private void OpenCell(int surveyId, int quotaId, List<int> cellIds, BvSurveyQuotaEntity quota, BvSurveyQuotaCellEntity cell)
        {
            var batchParameters = new FilteredByOpenedQuotaCellBatchParameters(surveyId, quotaId, cellIds);

            IAsyncOperationParameters parameters;
            string title;

            switch (_systemSettings.FCD.AlgorithmType)
            {
                case FcdAlgorithmType.DeleteCalls:
                    return;

                case FcdAlgorithmType.DisableCallsWithReenabling:
                    parameters = new AsyncOperations.Operations.CallsManagementOperations.UpdateFcdStatusOfCalls.Parameters
                    {
                        SurveyId = surveyId,
                        BatchParameters = batchParameters
                    };

                    if (quota != null && cell != null)
                    {
                        title = $"Update FCD state of calls for opened quota \"{quota.Name}\" cell \"{GetCellInformation(cell)}\"";
                    }
                    else
                    {
                        title = "Update FCD state of calls for opened quota cell";
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }

            ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                    0,
                    title,
                    false,
                    parameters,
                    AsyncOperationConstants.NormalPriority,
                    _supervisorNameProvider.Name);
        }

        public void OnQuotaUpdate(int surveyId, int quotaId)
        {
            using (var dbLock = ExclusiveDatabaseLock.CreateLock(DatabaseLockTimeoutsAndRecourceNames.GetFcdResourceName(surveyId), "OnQuotaUpdate", _databaseLockTimeouts.DefaultLockTimeoutInMs))
            {
                dbLock.EnterLock();

                ImportSurveyQuota(surveyId, quotaId);
            }

            EnqueueCallManagementOperationOnQuotaUpdate(surveyId, quotaId);
        }

        private void EnqueueCallManagementOperationOnQuotaUpdate(int surveyId, int quotaId)
        {
            IAsyncOperationParameters parameters;
            string title;

            var quota = quotaId > 0 ? _quotaRepository.TryGetById(surveyId, quotaId) : null;

            switch (_systemSettings.FCD.AlgorithmType)
            {
                case FcdAlgorithmType.DeleteCalls:

                    parameters = new AsyncOperations.Operations.CallsManagementOperations.DeleteCalls.Parameters
                    {
                        SurveyId = surveyId,
                        NewITS = (int)CallOutcome.FilteredByCallDelivery,
                        BatchParameters = new FilteredByClosedQuotaCellBatchParameters(surveyId, quotaId, new List<int>())
                    };

                    title = quota != null
                        ? $"Delete calls for closed quota \"{quota.Name}\" cells"
                        : "Delete calls for closed quota cells";

                    break;

                case FcdAlgorithmType.DisableCallsWithReenabling:

                    parameters = new AsyncOperations.Operations.CallsManagementOperations.UpdateFcdStatusOfCalls.
                        Parameters
                    {
                        SurveyId = surveyId,
                        BatchParameters = new FilteredBatchParameters(surveyId, 0, 0, CallStates.All, null)
                    };

                    title = quota != null
                        ? $"Update status of calls on quota \"{quota.Name}\" update"
                        : "Update status of calls";

                    break;

                default:
                    throw new NotImplementedException();
            }

            ServiceLocator.Resolve<IAsyncOperationQueue>().Enqueue(
                0,
                title,
                false,
                parameters,
                AsyncOperationConstants.NormalPriority,
                null);
        }

        public void OnLaunchSurvey(int surveyId, bool runForceImport, CancellationToken cancellationToken)
        {
            using (var dbLock = ExclusiveDatabaseLock.CreateLock(DatabaseLockTimeoutsAndRecourceNames.GetFcdResourceName(surveyId), "OnLaunchSurvey", _databaseLockTimeouts.DefaultLockTimeoutInMs))
            {
                dbLock.EnterLock();

                if (runForceImport || IsQuotaSchemaChanged(surveyId))
                    ImportSurveyQuotas(surveyId, cancellationToken);
            }

            EnqueueCallManagementOperationOnQuotaUpdate(surveyId, 0);
        }

        public void OnDeleteSurvey(int surveyId)
        {
            _quotaRepository.DeleteAll(surveyId);

            EventDetailsScope.Current.AddTiming($"Survey quotas deleted. Survey id: {surveyId}");
        }

        private QuotaCellsUpdateResult ImportSurveyQuotaCell(int surveyId, int quotaId, int cellId)
        {
            var survey = _surveyRepository.GetById(surveyId);
            //check where is quota storage
            if (survey.IsQuotaInCatiDb != true) return new QuotaCellsUpdateResult();

            using (var transaction = new DatabaseTransactionScope(
                new DatabaseTransactionOptions($"QuotaService.ImportQuotaCell")))
            {
                var quota = _seQuotaRepository.GetById(surveyId, quotaId);
                var quotaCell = _seQuotaCellRepository.GetById(surveyId, quotaId, cellId, quota.Data.FieldNames);
                _quotaCellRepository.Merge(quotaCell);

                var result = _quotaCellUpdater.UpdateQuotaCells(surveyId, quotaId);

                transaction.Commit();

                EventDetailsScope.Current.AddTiming($"Quota cell imported: {quotaCell}");

                return result;
            }
        }

        private void ImportSurveyQuota(int surveyId, int quotaId)
        {
            var survey = _surveyRepository.GetById(surveyId);
            //check where is quota storage
            if (survey.IsQuotaInCatiDb != true) return;

            using (var transaction = new DatabaseTransactionScope(
                new DatabaseTransactionOptions($"QuotaService.{nameof(ImportSurveyQuota)}")))
            {
                var quota = _seQuotaRepository.GetById(surveyId, quotaId);
                _quotaRepository.Merge(quota);
                var quotaCells = _seQuotaCellRepository.GetAllByQuota(surveyId, quotaId, quota.Data.FieldNames)
                    .ToList();
                _quotaCellRepository.Delete(surveyId, new[] { quotaId });
                _quotaCellRepository.Insert(quotaCells);

                _quotaCellUpdater.UpdateQuotaCells(surveyId, quotaId);

                _interviewQuotaCellService.Populate(surveyId, quotaId);

                transaction.Commit();

                EventDetailsScope.Current.AddTiming($"Quota imported. Cell count: {quotaCells.Count}");
            }
        }

        private bool IsQuotaSchemaChanged(int surveyId)
        {
            var quotas = _seQuotaRepository.GetAll(surveyId)
                .OrderBy(x => x.QuotaID).ToList();
            var quotaCells = quotas.SelectMany(
                quota => _seQuotaCellRepository.GetAllByQuota(surveyId, quota.QuotaID, quota.Data.FieldNames))
                .OrderBy(x => x.QuotaID).ThenBy(x => x.CellID).ToList();

            var oldQuotas = _quotaRepository.GetAll(surveyId)
                .OrderBy(x => x.QuotaID).ToList();
            var oldQuotaCells = _quotaCellRepository.GetBySurveyId(surveyId).Where(x => x.CellID > 0)
                .OrderBy(x => x.QuotaID).ThenBy(x => x.CellID).ToList();

            if (!quotas.SequenceEqual(oldQuotas))
                return true;

            if (!AreQuotaCellsEquals(oldQuotaCells, quotaCells))
                return true;

            return false;
        }

        private bool AreQuotaCellsEquals(List<BvSurveyQuotaCellEntity> oldCells, List<BvSurveyQuotaCellEntity> newCells)
        {
            if (oldCells.Count != newCells.Count)
                return false;

            for (int i = 0; i < oldCells.Count; i++)
            {
                if (oldCells[i].CellID != newCells[i].CellID)
                    return false;
                if (oldCells[i].IsDisabled != newCells[i].IsDisabled)
                    return false;
                if (oldCells[i].Limit != newCells[i].Limit)
                    return false;
                if (oldCells[i].LiveLimit != newCells[i].LiveLimit)
                    return false;
                if (oldCells[i].QuotaID != newCells[i].QuotaID)
                    return false;
                if (oldCells[i].SurveyID != newCells[i].SurveyID)
                    return false;
                if (oldCells[i].XmlData != newCells[i].XmlData)
                    return false;
            }

            return true;
        }

        private void ImportSurveyQuotas(int surveyId, CancellationToken cancellationToken)
        {
            var quotas = _seQuotaRepository.GetAll(surveyId).ToList();
            var quotaCells = quotas.SelectMany(
                quota => _seQuotaCellRepository.GetAllByQuota(surveyId, quota.QuotaID, quota.Data.FieldNames)).ToList();

            var fcdQuotaCount = quotas.Count(x => x.IsFCD == 1);
            var interviewCount = SurveyRepository.GetInterviewsCount(surveyId);
            cancellationToken.ThrowIfCancellationRequested();
            var interviewQuotaCellCount = fcdQuotaCount * interviewCount;
            if (interviewQuotaCellCount > _systemSettings.FCD.InterviewQuotaCellsTransactionThreshold)
            {
                ImportSurveyQuotasInternal(surveyId, quotas, quotaCells, cancellationToken);
            }
            else
            {
                using (var transaction = new DatabaseTransactionScope(
                           new DatabaseTransactionOptions($"QuotaService.{nameof(ImportSurveyQuotas)}")))
                {
                    ImportSurveyQuotasInternal(surveyId, quotas, quotaCells, cancellationToken);

                    transaction.Commit();
                }
            }

            EventDetailsScope.Current.AddTiming(
                $"Quotas imported. Quota count: {quotas.Count}, cell count: {quotaCells.Count}, interview quota cell count: {interviewQuotaCellCount}");
        }

        private void ImportSurveyQuotasInternal(int surveyId, List<BvSurveyQuotaEntity> quotas, List<BvSurveyQuotaCellEntity> quotaCells, CancellationToken cancellationToken)
        {
            var survey = _surveyRepository.GetById(surveyId);

            survey.IsQuotaInCatiDb = quotas.Count > 0;

            cancellationToken.ThrowIfCancellationRequested();

            _surveyRepository.Update(survey);

            _quotaRepository.DeleteAll(surveyId);
            cancellationToken.ThrowIfCancellationRequested();

            _quotaRepository.Insert(quotas);
            cancellationToken.ThrowIfCancellationRequested();

            _quotaCellRepository.DeleteAll(surveyId);
            cancellationToken.ThrowIfCancellationRequested();
            _quotaCellRepository.Insert(quotaCells);
            cancellationToken.ThrowIfCancellationRequested();

            UpdateAnyCells(surveyId);
            cancellationToken.ThrowIfCancellationRequested();

            _interviewQuotaCellService.Populate(surveyId, cancellationToken);
        }

        public void OnQuotaCellsChanged(
            int surveySid,
            int quotaSid,
            int[] openedCfCellIds,
            int[] closedCfCellIds,
            int[] optimisticallyClosedCfCellIds)
        {
            var countOfHardCellOperations = closedCfCellIds.Length;

            if (_systemSettings.FCD.AlgorithmType == FcdAlgorithmType.DisableCallsWithReenabling)
            {
                countOfHardCellOperations += optimisticallyClosedCfCellIds.Length;
                countOfHardCellOperations += openedCfCellIds.Length;
            }

            if (countOfHardCellOperations > ServiceLocator.Resolve<ISystemSettings>().Quotas.MaxQuestionsPerQuota)
            {
                OnQuotaUpdate(surveySid, quotaSid);
            }
            else
            {
                foreach (var cellId in openedCfCellIds)
                {
                    OnQuotaCellChanged(surveySid,
                        quotaSid,
                        cellId,
                        QuotaCellState.PessimisticallyOpened);
                }

                foreach (var cellId in closedCfCellIds)
                {
                    OnQuotaCellChanged(surveySid,
                        quotaSid,
                        cellId,
                        QuotaCellState.PessimisticallyClosed);
                }

                if (_systemSettings.FCD.AlgorithmType == FcdAlgorithmType.DisableCallsWithReenabling)
                {
                    foreach (int cellId in optimisticallyClosedCfCellIds)
                    {
                        OnQuotaCellChanged(surveySid,
                            quotaSid,
                            cellId,
                            QuotaCellState.OptimisticallyClosed);
                    }
                }
            }
        }

        public void OnQuotaCellsStateChanged(int surveySid, int quotaSid, List<CatiQuotaCellCountersState> quotaCellsCountersStates)
        {
            if (quotaCellsCountersStates.Count > _systemSettings.Quotas.MaxQuestionsPerQuota)
            {
                OnQuotaUpdate(surveySid, quotaSid);
            }
            else
            {
                foreach (var cellState in quotaCellsCountersStates)
                {
                    var quotaCellState = GetState(cellState.ActualCounters);
                    if (quotaCellState == QuotaCellState.OptimisticallyClosed)
                    {
                        if (_systemSettings.FCD.AlgorithmType == FcdAlgorithmType.DisableCallsWithReenabling)
                        {
                            OnQuotaCellChanged(surveySid,
                                quotaSid,
                                cellState.CellId,
                                quotaCellState);
                        }
                    }
                    else
                    {
                        OnQuotaCellChanged(surveySid,
                            quotaSid,
                            cellState.CellId,
                            quotaCellState);
                    }


                }
            }
        }

        public static QuotaCellState GetState(CatiQuotaCellCounters counters)
        {
            if (counters.Disabled)
            {
                return QuotaCellState.PessimisticallyClosed;
            }

            if (counters.Counter + counters.LiveCounter < counters.Limit)
            {
                return QuotaCellState.PessimisticallyOpened;
            }

            if (counters.Counter < counters.Limit && counters.Limit <= counters.Counter + counters.LiveCounter &&
                counters.Counter + counters.LiveCounter < counters.LiveLimit)
            {
                return QuotaCellState.OptimisticallyOpened;
            }

            if (counters.Counter < counters.Limit && counters.LiveLimit <= counters.Counter + counters.LiveCounter)
            {
                return QuotaCellState.OptimisticallyClosed;
            }

            if (counters.Counter >= counters.Limit)
            {
                return QuotaCellState.PessimisticallyClosed;
            }

            throw new ArgumentOutOfRangeException();
        }

        public QuotaInfo[] GetQuotaInfos(int surveyId)
        {
            return _seQuotaRepository
                .GetAll(surveyId)
                .Select(x => new QuotaInfo()
                {
                    Id = x.QuotaID,
                    Name = x.Name,
                    Table = x.TableName,
                    Fields = x.Data.FieldNames
                })
                .ToArray();
        }

        public string[] GetQuotaFields(int surveyId, int quotaId)
        {
            return _seQuotaRepository.TryGetById(surveyId, quotaId)?.Data.FieldNames ?? new string[] { };
        }

        public string[] GetQuotaFields(int surveyId, string quotaName)
        {
            return _seQuotaRepository.TryGetByName(surveyId, quotaName)?.Data.FieldNames ?? new string[] { };
        }

        public string GetQuotaName(int surveyId, int quotaId)
        {
            return _seQuotaRepository.TryGetById(surveyId, quotaId)?.Name;
        }

        public string GetQuotaTable(BvSurveyEntity survey, int quotaId)
        {
            return _seQuotaRepository.TryGetById(survey.SID, quotaId)?.TableName;
        }

        public string GetQuotaTable(BvSurveyEntity survey, string name)
        {
            return _seQuotaRepository.TryGetByName(survey.SID, name)?.TableName;
        }

        public bool IsExists(BvSurveyEntity survey, string quotaName)
        {
            return GetQuotaTable(survey, quotaName) != null;
        }

        public static string GetCellWhereForRepicationTable(string tableName, string[] quotaFields, string[] cellValues)
        {
            if (quotaFields.Length <= 0 || quotaFields.Length != cellValues.Length)
            {
                throw new ArgumentException("Wrong arguments");
            }

            return string.Join(" AND ", quotaFields.Select((x, i) => $"[{tableName}].[{x}] = '{cellValues[i]}'"));
        }

        public string[] GetCellValues(int surveyId, int quotaId, int cellId, string[] fields)
        {
            var cell = _seQuotaCellRepository.GetById(surveyId, quotaId, cellId, fields);
            return cell.Data.FieldValues.Select(x => x.Value).ToArray();
        }

        public Dictionary<string, string> GellQuotaCellValuesMap(string projectId, string quotaName)
        {
            var cellValuesMap = new Dictionary<string, string>();

            var quotaList = _authoringService.GetQuotaList(projectId, quotaName, QuotaMode.DesignWithProductionCounter);
            var fields = _authoringService.GetQuotaForms(projectId, quotaName).OfType<SingleForm>().ToList();

            foreach (QuotaRow row in quotaList.QuotaRows)
            {
                var codes = new List<string>();
                var labels = new List<string>();

                for (int i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];
                    var answer = field.SingleAnswers.Items.FirstOrDefault(x => x.Precode == row.FieldPrecodes[i]) as Answer;

                    labels.Add(answer != null ? answer.Texts.First().Value : String.Empty);
                    codes.Add(answer != null ? answer.Precode : String.Empty);
                }

                cellValuesMap.Add(string.Join(",", codes), string.Join(", ", labels));
            }
            return cellValuesMap;
        }

        public bool HasQuotas(int surveyId)
        {
            return _seQuotaRepository.GetAll(surveyId).Any();
        }

        public string GetClusterCellIdQuery(int surveySid, string tableAlias, string[] fields)
        {

            var cellNameQuery = GetClusterCellNameQuery(tableAlias, fields);

            return String.Format("ISNULL( ( SELECT CellId FROM BvClusteredQuotaCell cqc WHERE cqc.SurveyId = {0} AND cqc.Name = {1} ), 0 )", surveySid, cellNameQuery);
        }

        public string GetClusterCellNameQuery(string tableAlias, string[] fields)
        {
            if (fields.Length == 0)
                return "''";

            return String.Join(" + ",
                fields.Select(x => String.Format("'{0}=' + ISNULL( CAST( {1}.{0} AS NVARCHAR(MAX)),'' )", x, tableAlias)).ToArray());
        }

        public string GetCellInfo(int surveyId, int quotaId, int cellId)
        {
            var cell = _quotaCellRepository.TryGetById(surveyId, quotaId, cellId);

            return GetCellInformation(cell);
        }
    }
}