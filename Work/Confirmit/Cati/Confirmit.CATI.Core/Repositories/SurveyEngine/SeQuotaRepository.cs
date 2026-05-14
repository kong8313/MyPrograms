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
    public class SeQuotaRepository : ISeQuotaRepository
    {
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;

        public SeQuotaRepository(ISurveyDatabaseEngine surveyDatabaseEngine)
        {
            _surveyDatabaseEngine = surveyDatabaseEngine;
        }

        public BvSurveyQuotaEntity GetById(int surveyId, int quotaId)
        {
            var result = TryGetById(surveyId, quotaId);

            if (result == null)
            {
                throw ExceptionManager.NewInternalErrorException(
                    "Quota entity for survey id {0} and quota id {1} not found.",
                    surveyId, quotaId);
            }

            return result;
        }

        public BvSurveyQuotaEntity TryGetById(int surveyId, int quotaId)
        {
            string query = @"SELECT * 
                    FROM <Schema>.quotas
                    WHERE quotaid = @QuotaId
                        AND iscati > 0";
            // iscati = 0 -> No quota in cati at all

            return _surveyDatabaseEngine.ExecuteScalarList(surveyId, query,
                    row => GetQuotaEntity(surveyId, row),
                    new SqlParameter("@QuotaId", quotaId))
                .FirstOrDefault();
        }

        public BvSurveyQuotaEntity GetByName(int surveyId, string quotaName)
        {
            var result = TryGetByName(surveyId, quotaName);

            if (result == null)
            {
                throw ExceptionManager.NewInternalErrorException(
                    "Quota entity for survey id {0} and quota name {1} not found.",
                    surveyId, quotaName);
            }

            return result;
        }

        public BvSurveyQuotaEntity TryGetByName(int surveyId, string quotaName)
        {
            string query = @"SELECT * 
                    FROM <Schema>.quotas
                    WHERE quotaname = @QuotaName
                        AND iscati > 0";
            // iscati = 0 -> No quota in cati at all

            return _surveyDatabaseEngine.ExecuteScalarList(surveyId, query,
                    row => GetQuotaEntity(surveyId, row),
                    new SqlParameter("@QuotaName", quotaName))
                .FirstOrDefault();
        }

        public IEnumerable<BvSurveyQuotaEntity> GetAll(int surveyId)
        {
            string query = @"SELECT * 
                    FROM <Schema>.quotas
                    WHERE iscati > 0";
            // iscati = 0 -> No quota in cati at all

            return _surveyDatabaseEngine.ExecuteScalarList(surveyId, query,
                row => GetQuotaEntity(surveyId, row));
        }

        private BvSurveyQuotaEntity GetQuotaEntity(int surveyId, IDataReader row)
        {
            var quotaId = row.GetValueOrDefault("quotaid", 0);
            var tableName = row.GetValueOrDefault<string>("tablename");
            return new BvSurveyQuotaEntity() {
                SurveyID = surveyId,
                QuotaID = quotaId,
                Name = row.GetValueOrDefault<string>("quotaname"),
                TableName = tableName,
                Email = row.GetValueOrDefault<string>("email"),
                // iscati = 0 -> No quota in cati at all
                // iscati = 1 -> IsFCD = 1
                // iscati = 2 -> IsFCD = 0
                IsFCD = row.GetValueOrDefault("iscati", 0) == 1 ? 1 : 0,
                IsOptimistic = row.GetValueOrDefault("is_optimistic", false),
                Data = new QuotaData {
                    FieldNames = TryGetQuotaFieldsById(surveyId, quotaId, tableName).ToArray()
                }
            };
        }

        private IEnumerable<string> GetQuotaFieldsById(int surveyId, int quotaId, string tableName)
        {
            var result = TryGetQuotaFieldsById(surveyId, quotaId, tableName);

            if (result == null)
            {
                throw ExceptionManager.NewInternalErrorException(
                    "Quota fields for survey id {0} and quota id {1} not found.",
                    surveyId, quotaId);
            }

            return result;
        }

        private IEnumerable<string> TryGetQuotaFieldsById(int surveyId, int quotaId, string tableName)
        {
            var orderedFieldNamesQuery = @"
                SELECT COLUMN_NAME
                FROM INFORMATION_SCHEMA.COLUMNS
                INNER JOIN <Schema>.quota_field ON COLUMN_NAME = fieldname
                WHERE 
                    TABLE_NAME = @TableName AND TABLE_SCHEMA = '<Schema>' AND quotaid = @QuotaId";

            return _surveyDatabaseEngine.ExecuteScalarList<string>(surveyId, orderedFieldNamesQuery,
                new SqlParameter("@QuotaId", quotaId),
                new SqlParameter("@TableName", tableName));
        }
    }
}