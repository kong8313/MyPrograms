using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.DAL.Generated.Entity.TableType;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Repositories
{
    public class QuotaCellRepository : IQuotaCellRepository
    {
        public IReadOnlyCollection<BvSurveyQuotaCellEntity> GetQuotaCells(int surveyId, int quotaId)
        {
            var result = BvSurveyQuotaCellAdapter.GetByCondition(
                "[SurveyId] = @SurveyId AND [QuotaId] = @QuotaId",
                new SqlParameter("@SurveyId", surveyId),
                new SqlParameter("@QuotaId", quotaId));

            return result;
        }
        
        public BvSurveyQuotaCellEntity TryGetById(int surveyId, int quotaId, int cellId)
        {
            var result = BvSurveyQuotaCellAdapter.GetByCondition(
                "[SurveyId] = @SurveyId AND [QuotaId] = @QuotaId AND [CellId] = @CellId",
                new SqlParameter("@SurveyId", surveyId),
                new SqlParameter("@QuotaId", quotaId),
                new SqlParameter("@CellId", cellId)).FirstOrDefault();

            return result;
        }

        public void Merge([NotNull] BvSurveyQuotaCellEntity cell)
        {
            if (cell.SurveyID == 0)
                throw ExceptionManager.NewArgumentException(nameof(cell.SurveyID));
            if (cell.QuotaID == 0)
                throw ExceptionManager.NewArgumentException(nameof(cell.QuotaID));
            if (cell.CellID == 0)
                throw ExceptionManager.NewArgumentException(nameof(cell.CellID));

            BvSurveyQuotaCellAdapter.Merge(cell);
        }

        private const int ImportBatchSize = 10000;
        private const int ImportBulkTimeout = 60 * 10;

        public void Insert(List<BvSurveyQuotaCellEntity> cells)
        {
            InsertCellsBulk(cells);
        }

        public List<BvSurveyQuotaCellEntity> GetBySurveyId(int surveyId)
        {
            var query = "[SurveyId] = @SurveyId";

            return BvSurveyQuotaCellAdapter.GetByCondition(query, new SqlParameter("@SurveyId", surveyId));
        }

        public List<BvSurveyQuotaCellEntity> GetCells(int surveyId, int quotaId)
        {
            var query = "[SurveyId] = @SurveyId AND [QuotaId] = @QuotaId";

            return BvSurveyQuotaCellAdapter.GetByCondition(query,
                new SqlParameter("@SurveyId", surveyId),
                new SqlParameter("@QuotaId", quotaId));
        }

        private static void InsertCellsBulk(List<BvSurveyQuotaCellEntity> cells)
        {
            var bulkTable = BvSurveyQuotaCellAdapter.CreateDataTable();
            DatabaseTools.BulkAdd(
                bulkTable,
                BvSurveyQuotaCellAdapter.SaveEntity2DataTable,
                cells,
                ImportBatchSize,
                ImportBulkTimeout);
        }

        public void Delete(int surveyId, IEnumerable<int> quotaIds)
        {
            if (surveyId == 0)
                throw ExceptionManager.NewArgumentException(nameof(surveyId));

            if (quotaIds == null) return;

            var quotaIdsForDeletion = string.Join(", ", quotaIds);
            if (string.IsNullOrWhiteSpace(quotaIdsForDeletion)) return;

            BvSurveyQuotaCellAdapter.DeleteByCondition(
                $"[SurveyId] = @SurveyId AND [QuotaId] in ({quotaIdsForDeletion})",
                new SqlParameter("@SurveyId", surveyId));
        }

        public void DeleteAll(int surveyId)
        {
            if (surveyId == 0)
                throw ExceptionManager.NewArgumentException(nameof(surveyId));

            BvSurveyQuotaCellAdapter.DeleteByCondition(
                "[SurveyId] = @SurveyId",
                new SqlParameter("@SurveyId", surveyId));
        }

        private Dictionary<string, BvSurveyQuotaCellTypeEntity> GetCellsDictionary(List<BvSurveyQuotaCellEntity> cells)
        {
            return cells.Select(x => SurveyQuotaCellEntityToType(x)).ToDictionary(x => GetKeyForSurveyQuotaCellEntity(x));
        }

        private string GetKeyForSurveyQuotaCellEntity(BvSurveyQuotaCellTypeEntity cell)
        {
            return $"{cell.SurveyID}_{cell.QuotaID}_{cell.CellID}";
        }

        private BvSurveyQuotaCellTypeEntity SurveyQuotaCellEntityToType(BvSurveyQuotaCellEntity cell)
        {
            return new BvSurveyQuotaCellTypeEntity()
            {
                SurveyID = cell.SurveyID,
                QuotaID = cell.QuotaID,
                CellID = cell.CellID,
                Counter = cell.Counter,
                IsDisabled = cell.IsDisabled,
                IsOpen = cell.IsOpen,
                Limit = cell.Limit,
                LiveCounter = cell.LiveCounter,
                LiveLimit = cell.LiveLimit,
                XmlData = cell.XmlData
            };
        }

        public void MergeAnyCells(int surveyId, int quotaId, List<BvSurveyQuotaCellEntity> cells)
        {
            var changedCells = GetCellsDictionary(cells);
            var existedCells = GetCellsDictionary(GetCells(surveyId, quotaId).Where(x => x.CellID < 0).ToList());

            var cellsToUpdate = new List<BvSurveyQuotaCellTypeEntity>();
            var cellIdsForDeletion = new List<int>();

            foreach (var cell in existedCells)
            {
                if (!changedCells.ContainsKey(cell.Key))
                {
                    cellIdsForDeletion.Add(cell.Value.CellID);
                }
            }

            foreach (var cell in changedCells)
            {
                if (!existedCells.ContainsKey(cell.Key) || !AreCellsEqual(existedCells[cell.Key], cell.Value))
                {
                    cellsToUpdate.Add(cell.Value);
                }
            }

            if (cellsToUpdate.Count > 0)
            {
                var updateCellsQuery = @"
MERGE [BvSurveyQuotaCell] AS TARGET
USING @temp AS SOURCE
ON (TARGET.[SurveyID] = SOURCE.[SurveyID] AND TARGET.[QuotaID] = SOURCE.[QuotaID] AND TARGET.[CellID] = SOURCE.[CellID])
WHEN MATCHED 
THEN UPDATE SET TARGET.[Counter] = SOURCE.[Counter], TARGET.[Limit] = SOURCE.[Limit],
TARGET.[LiveCounter] = SOURCE.[LiveCounter], TARGET.[LiveLimit] = SOURCE.[LiveLimit],
TARGET.[IsDisabled] = SOURCE.[IsDisabled], TARGET.[IsOpen] = SOURCE.[IsOpen], TARGET.[XmlData] = SOURCE.[XmlData]
WHEN NOT MATCHED BY TARGET
THEN INSERT VALUES (SOURCE.[SurveyID], SOURCE.[QuotaID], SOURCE.[CellID], SOURCE.[Counter], SOURCE.[Limit], SOURCE.[LiveCounter], SOURCE.[LiveLimit],
SOURCE.[IsDisabled], SOURCE.[IsOpen], SOURCE.[XmlData]);
";

                new DatabaseEngine().ExecuteNonQuery(updateCellsQuery, BvSurveyQuotaCellTypeAdapter.CreateSqlParameter("@temp", cellsToUpdate));
            }

            if (cellIdsForDeletion.Count > 0)
            {
                BvSurveyQuotaCellAdapter.DeleteByCondition(
                    $"[SurveyId] = @SurveyId AND [QuotaId] = @QuotaId AND [CellID] in ({string.Join(", ", cellIdsForDeletion)})",
                    new SqlParameter("@SurveyId", surveyId),
                    new SqlParameter("@QuotaId", quotaId));
            }
        }

        private bool AreCellsEqual(BvSurveyQuotaCellTypeEntity cell1, BvSurveyQuotaCellTypeEntity cell2)
        {
            return cell1.SurveyID == cell2.SurveyID && cell1.QuotaID == cell2.QuotaID && cell1.CellID == cell2.CellID &&
                cell1.Counter == cell2.Counter && cell1.Limit == cell2.Limit && cell1.LiveCounter == cell2.LiveCounter &&
                cell1.LiveLimit == cell2.LiveLimit && cell1.IsDisabled == cell2.IsDisabled && cell1.IsOpen == cell2.IsOpen && cell1.XmlData == cell2.XmlData;
        }
    }
}
