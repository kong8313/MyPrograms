using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Adapter.TableType;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Confirmit.CATI.Core.Repositories
{
    public class InterviewQuotaCellRepository : IInterviewQuotaCellRepository
    {
        private const int BatchSize = 10000;
        private const int ImportBulkTimeout = 60 * 10;

        public List<BvInterviewQuotaCellEntity> GetByInterviewId(int surveyId, int interviewId)
        {
            return BvInterviewQuotaCellAdapter.GetByCondition(
                "SurveyId = @surveyId AND InterviewId = @interviewId",
                new SqlParameter("@surveyId", surveyId),
                new SqlParameter("@interviewId", interviewId));
        }

        public void Insert(List<BvInterviewQuotaCellEntity> cells)
        {
            InsertCellsBulk(cells);
        }

        public void Delete(int surveyId, List<int> interviewIds)
        {
            var batches = interviewIds.SplitIntoBatches(BatchSize);

            foreach (var ids in batches)
            {
                BvInterviewQuotaCellAdapter.DeleteByCondition(
                    "SurveyId = @surveyId AND EXISTS( SELECT 1 FROM @ids where Value = InterviewId )",
                    BvIntArrayTypeAdapter.CreateSqlParameter("@ids", ids),
                    new SqlParameter("@surveyId", surveyId));
            }
        }

        public void Delete(int surveyId)
        {
            var query = $"DELETE FROM dbo.[BvInterviewQuotaCell] WHERE [SurveyID] = {surveyId}";
            DatabaseTools.BulkRemove(query);
        }

        private static void InsertCellsBulk(List<BvInterviewQuotaCellEntity> cells)
        {
            var bulkTable = BvInterviewQuotaCellAdapter.CreateDataTable();
            DatabaseTools.BulkAdd(
                bulkTable,
                BvInterviewQuotaCellAdapter.SaveEntity2DataTable,
                cells,
                BatchSize,
                ImportBulkTimeout);
        }
    }
}
