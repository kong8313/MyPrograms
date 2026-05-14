using System;
using System.Collections.Generic;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.ReplicationServiceImplementation;

namespace Confirmit.CATI.IntegrationTests.Framework.Data.Builders
{
    public class DummySurveyDatabaseBuilder : ISurveyDatabaseBuilder
    {
        private readonly int _surveyId;

        protected DummySurveyDatabaseBuilder(int surveyId)
        {
            _surveyId = surveyId;
            ProjectId = SurveyRepository.GetById(surveyId).Name;
        }

        public void CreateQuotaTables()
        {
            throw new NotImplementedException();
        }

        public void CreateRespondentTable(IEnumerable<FormData> forms = null)
        {
            throw new NotImplementedException();
        }

        public void CreateResponseTable(string tableName, IEnumerable<FormData> forms)
        {
            throw new NotImplementedException();
        }

        public void CreateFormAndFieldTable(FormData[] forms)
        {
            throw new NotImplementedException();
        }

        public string ConnectionString
        {
            get { throw new NotImplementedException(); }
        }

        public void EnableChangeTracking(TableInfo[] tableInfo)
        {
            throw new NotImplementedException();
        }

        public void ClearQuotaTables()
        {
            throw new NotImplementedException();
        }

        public void CreateQuota(QuotaData quota, FormData[] formData)
        {
            throw new NotImplementedException();
        }

        public string ProjectId { get; set; }

        public void CloseCell(int quotaId, int cellId)
        {
            throw new NotImplementedException();
        }

        public void CloseCellOptimistically(int quotaId, int cellId)
        {
            throw new NotImplementedException();
        }

        public void OpenCell(int quotaId, int cellId)
        {
            throw new NotImplementedException();
        }

        private int _interviewId = 0;

        public int AddInterview(int batchId, string catiExtendedStatus, InterviewData interview)
        {
            return ++_interviewId;
        }

        public int CreateRespondent(int batchId, string catiExtendedStatus, InterviewData interview)
        {
            throw new NotImplementedException();
        }

        public void SetRespondentTableColumnValue(int[] respondentIds, string column, string value)
        {
            throw new NotImplementedException();
        }

        public void SetInterviewData(int respid, string data)
        {
            throw new NotImplementedException();
        }

        public string GetInterviewData(int respid, string requestedColumns)
        {
            throw new NotImplementedException();
        }

        public void SetBatchId(int respId, int batchId, int updateBatchId)
        {
            throw new NotImplementedException();
        }

        private static int currentBatchId = 1;

        public int GetNewBatchId()
        {
            return currentBatchId++;
        }

        public static ISurveyDatabaseBuilder Create(int surveyId)
        {
            return new DummySurveyDatabaseBuilder(surveyId);
        }

        public void DeleteRespondent(int respId)
        {
            throw new NotImplementedException();
        }
    }
}