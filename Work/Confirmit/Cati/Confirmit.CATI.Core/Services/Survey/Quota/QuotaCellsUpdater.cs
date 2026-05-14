using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public class QuotaCellsUpdateResult
    {
        public List<int> OpenedAnyCellsIds { get; set; }
        public List<int> ClosedAnyCellsIds { get; set; }

        public QuotaCellsUpdateResult()
        {
            OpenedAnyCellsIds = new List<int>();
            ClosedAnyCellsIds = new List<int>();
        }
    }

    public class QuotaCellsUpdater
    {
        private readonly ISystemSettings _systemSettings;
        private readonly IQuotaRepository _quotaRepository;
        private readonly IQuotaCellRepository _quotaCellRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IQuotaDatabaseReader _quotaDatabaseReader;

        public QuotaCellsUpdater(
            ISystemSettings systemSettings,
            IQuotaRepository quotaRepository,
            IQuotaCellRepository quotaCellRepository,
            ISurveyRepository surveyRepository,
            IQuotaDatabaseReader quotaDatabaseReader)
        {
            _systemSettings = systemSettings;
            _quotaRepository = quotaRepository;
            _quotaCellRepository = quotaCellRepository;
            _surveyRepository = surveyRepository;
            _quotaDatabaseReader = quotaDatabaseReader;
        }

        public QuotaCellsUpdateResult UpdateQuotaCells(int surveyId, int quotaId)
        {
            var quota = _quotaRepository.TryGetById(surveyId, quotaId);
            var isOptimistic = quota.IsOptimistic && _systemSettings.FCD.BehaviorType == 1;

            string[] fields = _quotaDatabaseReader.GetQuotaFields(surveyId, quotaId).ToArray();

            var precodesByFields = fields.ToDictionary(
                field => field, field => _quotaDatabaseReader.GetFieldPrecodes(surveyId, quotaId, field).ToArray());

            var quotaMatrix = new QuotaMatrix(precodesByFields.Values.ToArray());

            var cells = _quotaDatabaseReader
                .GetQuotaCells(surveyId, quotaId, fields, isOptimistic).ToDictionary(x => x.Id);
            var initialIsOpenValues = cells
                .ToDictionary(entry => entry.Key, entry => entry.Value.IsOpen);

            var keymap = MakeKeymap(surveyId, quotaId, fields);

            CalculateCellStatuses(quotaMatrix, cells, isOptimistic);

            UpdateCells(quotaMatrix, surveyId, quotaId, initialIsOpenValues);

            return UpdateAnyCells(quotaMatrix, fields, quotaId, surveyId, initialIsOpenValues);
        }

        private int[] MakeKeymap(int surveyId, int quotaId, string[] fields)
        {
            var replicationTableFields = _quotaDatabaseReader.GetAllFields(surveyId).ToArray();

            var keymap = new int[fields.Length];
            for (int index = 0; index < fields.Length; index++)
            {
                int keyIndex = Array.FindIndex(replicationTableFields, x => x == fields[index]);
                if (keyIndex < 0)
                    throw new Exception($"quota filed [{fields[index]}] for quota with id = {quotaId} not found in replication table for survey with sid = {surveyId}");

                keymap[index] = keyIndex;
            }

            return keymap;
        }

        private void CalculateCellStatuses(QuotaMatrix quotaMatrix, Dictionary<int, QuotaCellInfo> cells, bool isOptimistic)
        {
            foreach (var cell in cells.Values)
            {
                if (cell.Id > 0)
                {
                    cell.IsOpen = cell.Limit > cell.Counter;

                    if (cell.IsOpen && isOptimistic)
                        cell.IsOpen = cell.LiveLimit > (cell.Counter + cell.LiveCounter);

                    if (cell.IsDisabled)
                        cell.IsOpen = false;

                    quotaMatrix[cell.Key] = cell;
                    if (cell.IsOpen)
                        OnCellChanged(quotaMatrix, cell);
                }
            }
        }

        private QuotaCellsUpdateResult UpdateAnyCells(QuotaMatrix quotaMatrix, string[] fields, int quotaId, int surveyId, Dictionary<int, bool> initialIsOpenValues)
        {
            var openedAnyCellIds = new List<int>();
            var closedAnyCellIds = new List<int>();

            int anyCellId = -1;
            var anyCells = new List<BvSurveyQuotaCellEntity>();
            foreach (var cell in quotaMatrix.Cells)
            {
                if (cell.Key.Contains(null))
                {
                    var fieldValues = new List<QuotaCellFieldValue>();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        var fieldValue = new QuotaCellFieldValue
                        {
                            Field = fields[i],
                            Value = cell.Key[i] ?? string.Empty
                        };

                        fieldValues.Add(fieldValue);
                    }

                    var anyCell = new BvSurveyQuotaCellEntity
                    {
                        CellID = anyCellId--,
                        QuotaID = quotaId,
                        SurveyID = surveyId,
                        Counter = cell.Counter,
                        IsDisabled = cell.IsDisabled,
                        LiveLimit = cell.LiveLimit,
                        LiveCounter = cell.LiveCounter,
                        IsOpen = cell.IsOpen,
                        Limit = cell.Limit,
                        Data = new QuotaCellData
                        {
                            FieldValues = fieldValues.ToArray()
                        }
                    };

                    anyCells.Add(anyCell);

                    if (initialIsOpenValues.ContainsKey(anyCell.CellID))
                    {
                        if (initialIsOpenValues[anyCell.CellID] != anyCell.IsOpen)
                        {
                            if (anyCell.IsOpen)
                            {
                                openedAnyCellIds.Add(anyCell.CellID);
                            }
                            else
                            {
                                closedAnyCellIds.Add(anyCell.CellID);
                            }
                        }
                    }

                }
            }

            _quotaCellRepository.MergeAnyCells(surveyId, quotaId, anyCells);

            return new QuotaCellsUpdateResult()
            {
                OpenedAnyCellsIds = openedAnyCellIds,
                ClosedAnyCellsIds = closedAnyCellIds
            };
        }

        private void UpdateCells(QuotaMatrix quotaMatrix, int surveyId, int quotaId, Dictionary<int, bool> initialIsOpenValues)
        {
            var openedCellIds = new List<int>();
            var closedCellIds = new List<int>();

            foreach (var cell in quotaMatrix.Cells)
            {
                if (cell.Key.Contains(null) == false)
                {
                    if (initialIsOpenValues[cell.Id] != cell.IsOpen)
                    {
                        if (cell.IsOpen)
                        {
                            openedCellIds.Add(cell.Id);
                        }
                        else
                        {
                            closedCellIds.Add(cell.Id);
                        }
                    }

                }
            }

            var updateCellsQuery = $"UPDATE [BvSurveyQuotaCell] SET [IsOpen] = @isOpen WHERE [SurveyID] = {surveyId} AND [QuotaID] = {quotaId} AND EXISTS( SELECT 1 FROM @ids where Value = [CellID] )";
            if (openedCellIds.Count > 0)
            {
                new DatabaseEngine()
                    .ExecuteNonQuery(updateCellsQuery, BvIntArrayTypeAdapter.CreateSqlParameter("@ids", openedCellIds), new SqlParameter("@isOpen", 1));
            }

            if (closedCellIds.Count > 0)
            {
                new DatabaseEngine()
                    .ExecuteNonQuery(updateCellsQuery, BvIntArrayTypeAdapter.CreateSqlParameter("@ids", closedCellIds), new SqlParameter("@isOpen", 0));
            }
        }

        private void OnCellChanged(QuotaMatrix quotaMatrix, QuotaCellInfo cell)
        {
            string[] key = cell.Key.ToArray();// copy key

            if (cell.IsOpen)
            {
                //
                // if cell is being opened, then we should check parent dimensions on openning
                // 
                for (int index = 0; index < key.Length; index++)
                {
                    //save curent value for dimnesion 
                    string field = key[index];

                    // if aready unrecorded, then nothing to do
                    if (field == null)
                        continue;

                    // get unrecord cell
                    key[index] = null;
                    QuotaCellInfo unrecordCell = quotaMatrix[key];

                    // descrimet closed child cells
                    unrecordCell.Counter--;

                    // if unrecord cell still opened, then close it
                    if (unrecordCell.IsOpen == false)
                    {
                        unrecordCell.IsOpen = true;

                        OnCellChanged(quotaMatrix, unrecordCell);
                    }

                    // restore key
                    key[index] = field;
                }
            }
        }
    }
}
