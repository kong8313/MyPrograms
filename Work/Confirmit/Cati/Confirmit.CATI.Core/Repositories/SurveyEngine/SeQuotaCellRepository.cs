using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc.Extensions;
using Confirmit.CATI.Core.Repositories.SurveyEngine.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Survey.Quota;

namespace Confirmit.CATI.Core.Repositories.SurveyEngine
{
    public class SeQuotaCellRepository : ISeQuotaCellRepository
    {
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;

        public SeQuotaCellRepository(ISurveyDatabaseEngine surveyDatabaseEngine)
        {
            _surveyDatabaseEngine = surveyDatabaseEngine;
        }

        public BvSurveyQuotaCellEntity GetById(int surveyId, int quotaId, int cellId, IEnumerable<string> quotaFields)
        {
            var result = TryGetById(surveyId, quotaId, cellId, quotaFields);

            if (result == null)
            {
                throw ExceptionManager.NewInternalErrorException(
                    "Quota cell entity for survey id {0}, quota id {1} and cell id {2} not found.",
                    surveyId, quotaId, cellId);
            }

            return result;
        }

        public BvSurveyQuotaCellEntity TryGetById(int surveyId, int quotaId, int cellId, IEnumerable<string> quotaFields)
        {
            var quotaTableName = GetQuotaTableName(surveyId, quotaId);

            if (string.IsNullOrWhiteSpace(quotaTableName)) return null;

            string query = $@"SELECT * 
                    FROM <Schema>.{quotaTableName}
                    WHERE quotaid = @CellId";

            return _surveyDatabaseEngine.ExecuteScalarList(surveyId, query,
                    row => GetQuotaCellEntity(row, surveyId, quotaId, quotaFields.ToList()),
                    new SqlParameter("@CellId", cellId))
                .FirstOrDefault();
        }

        public IEnumerable<BvSurveyQuotaCellEntity> GetAllByQuota(int surveyId, int quotaId, IEnumerable<string> quotaFields)
        {
            var quotaTableName = GetQuotaTableName(surveyId, quotaId);

            if (string.IsNullOrWhiteSpace(quotaTableName)) return null;

            string query = $@"SELECT * FROM <Schema>.{quotaTableName}";

            return _surveyDatabaseEngine.ExecuteScalarList(surveyId, query,
                row => GetQuotaCellEntity(row, surveyId, quotaId, quotaFields.ToList()));
        }

        private string GetQuotaTableName(int surveyId, int quotaId)
        {
            string queryTableName = @"SELECT tablename 
                    FROM <Schema>.quotas
                    WHERE quotaid = @QuotaId
                        AND iscati > 0";
            var quotaTableName =
                _surveyDatabaseEngine.ExecuteScalar<string>(surveyId, queryTableName,
                    new SqlParameter("@QuotaId", quotaId));
            return quotaTableName;
        }

        private static BvSurveyQuotaCellEntity GetQuotaCellEntity(IDataReader row, int surveyId, int quotaId, List<string> fieldNames)
        {
            return new BvSurveyQuotaCellEntity()
            {
                SurveyID = surveyId,
                QuotaID = quotaId,
                CellID = (int)row["quotaid"],
                Counter = row.GetValueOrDefault("counter", 0),
                Limit = row.GetValueOrDefault("limit", 0),
                LiveCounter = row.GetValueOrDefault("live_counter", 0),
                LiveLimit = row.GetValueOrDefault("live_limit", 0),
                IsDisabled = row.GetValueOrDefault("disabled", false),
                Data = new QuotaCellData
                {
                    FieldValues = fieldNames.Select(fieldName => new QuotaCellFieldValue
                    {
                        Field = fieldName, Value = row.GetValueOrDefault<string>(fieldName)
                    }).ToArray()
                }
            };
        }
    }
}
