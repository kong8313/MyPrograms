using System.Collections.Generic;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public interface ISurveyDatabaseBuilder : ISurveyDatabase
    {
        void CreateQuotaTables();
        void CreateRespondentTable(IEnumerable<FormData> forms = null);
        void CreateResponseTable(string tableName, IEnumerable<FormData> forms);
        void CreateFormAndFieldTable(FormData[] forms);

        string ConnectionString { get; }
        void EnableChangeTracking(Core.Services.ReplicationServiceImplementation.TableInfo[] tableInfo);

        void ClearQuotaTables();
        void CreateQuota(QuotaData quota, FormData[] formData);
    }

    public interface ISurveyDatabase
    {
        string ProjectId { get; }
        void CloseCell(int quotaId, int cellId);
        void CloseCellOptimistically(int quotaId, int cellId);
        void OpenCell(int quotaId, int cellId);
        int AddInterview(int batchId, string catiExtendedStatus, InterviewData interview);
        void DeleteRespondent(int respId);
        void SetRespondentTableColumnValue(int[] respondentIds, string column, string value);
        void SetInterviewData(int respid, string data);
        string GetInterviewData(int respid, string requestedColumns);
        void SetBatchId(int respId, int batchId, int updateBatchId);
        int CreateRespondent(int batchId, string catiExtendedStatus, InterviewData interview);
        int GetNewBatchId();
    }
}
