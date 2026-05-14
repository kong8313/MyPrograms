using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Core.Services.Survey.Quota
{
    public class QuotaDatabaseReader : IQuotaDatabaseReader
    {
        private readonly DatabaseEngine _databaseEngine;

        public QuotaDatabaseReader()
        {
            _databaseEngine = new DatabaseEngine();
        }

        public IEnumerable<string> GetAllFields(int surveySid)
        {
            var selectFieldNamesSql =
                @"SELECT [XmlData] 
                  FROM dbo.[BvSurveyQuota]
                  WHERE [SurveyID] = @SurveyID";
            DataTable table = _databaseEngine.ExecuteDataTable<DataTable>(
                selectFieldNamesSql,
                CommandType.Text,
                new SqlParameter("@SurveyID", surveySid));

            var result = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                var xmlData = XmlSerialization.Deserialize<QuotaData>((string)row["XmlData"]);
                result.AddRange(xmlData.FieldNames);
            }

            return result.Distinct();
        }

        public IEnumerable<ClrQuotaInfo> GetQuotas(int surveySid)
        {
            var selectFieldsSql =
                @"SELECT *, (case when IsFCD = 1 then 1 else 0 end) isFcdValue
                      FROM dbo.[BvSurveyQuota]
                      WHERE [SurveyID] = @SurveyID
                      ORDER BY [QuotaID]";
            DataTable table = _databaseEngine.ExecuteDataTable<DataTable>(
                selectFieldsSql,
                CommandType.Text,
                new SqlParameter("@SurveyID", surveySid));
            var isSupportOptimisticQuotas = table.Columns.Contains("IsOptimistic");

            return from DataRow row in table.Rows
                   select new ClrQuotaInfo
                   {
                       Id = (int)row["QuotaID"],
                       Name = (string)row["Name"],
                       TableName = (string)row["TableName"],
                       IsFcd = (int)row["isFcdValue"] == 1,
                       IsOptimistic = isSupportOptimisticQuotas && (bool)row["IsOptimistic"]
                   };
        }

        public IEnumerable<string> GetQuotaFields(int surveySid, int quotaId)
        {
            //
            // Read quota fields info.
            // For it we should read data from quota_field in CF DB.
            // This table contains following data:
            // quotaid - id of quota
            // fieldname - field name is used in quota
            //
            var selectFieldNamesSql =
                @"SELECT [XmlData] 
                  FROM dbo.[BvSurveyQuota]
                  WHERE [SurveyID] = @SurveyID AND [QuotaID] = @QuotaID";
            DataTable table = _databaseEngine.ExecuteDataTable<DataTable>(
                selectFieldNamesSql,
                CommandType.Text,
                new SqlParameter("@SurveyID", surveySid),
                new SqlParameter("@QuotaID", quotaId));

            if (table.Rows.Count == 0 || table.Rows[0]["XmlData"] is DBNull || table.Rows[0]["XmlData"] == null)
            {
                return new string[0];
            }

            var xmlData = XmlSerialization.Deserialize<QuotaData>((string)table.Rows[0]["XmlData"]);
            return xmlData.FieldNames;
        }

        public Dictionary<string, HashSet<string>> GetFieldPrecodes(int surveySid, int quotaId)
        {
            var selectFieldsSql =
              @"SELECT [XmlData] 
                  FROM dbo.[BvSurveyQuotaCell]
                  WHERE [SurveyID] = @SurveyID AND [QuotaID] = @QuotaID AND [CellID] > 0";
            DataTable table = _databaseEngine.ExecuteDataTable<DataTable>(
                    selectFieldsSql,
                    CommandType.Text,
            new SqlParameter("@SurveyID", surveySid),
            new SqlParameter("@QuotaID", quotaId));

            var availableValues = new Dictionary<string, HashSet<string>>();
            foreach (DataRow row in table.Rows)
            {
                var cellData = XmlSerialization.Deserialize<QuotaCellData>((string)row["XmlData"]);
                foreach (var fieldvalue in cellData.FieldValues)
                {
                    if (!availableValues.ContainsKey(fieldvalue.Field))
                        availableValues.Add(fieldvalue.Field, new HashSet<string>());

                    availableValues[fieldvalue.Field].Add(fieldvalue.Value);
                }
            }

            return availableValues;
        }

        public IEnumerable<string> GetFieldPrecodes(int surveySid, int quotaId, string fieldName)
        {
            var selectFieldsSql =
                @"SELECT [XmlData] 
                  FROM dbo.[BvSurveyQuotaCell]
                  WHERE [SurveyID] = @SurveyID AND [QuotaID] = @QuotaID AND [CellID] > 0";
            DataTable table = _databaseEngine.ExecuteDataTable<DataTable>(
                    selectFieldsSql,
                    CommandType.Text,
            new SqlParameter("@SurveyID", surveySid),
            new SqlParameter("@QuotaID", quotaId));

            bool isUnrecordPrecodeAdded = false;
            var keys = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                var cellData = XmlSerialization.Deserialize<QuotaCellData>((string)row["XmlData"]);
                var value = cellData.FieldValues.FirstOrDefault(f => f.Field == fieldName)?.Value;
                if (value == null)
                    isUnrecordPrecodeAdded = true;
                keys.Add(value);
            }
            if (!isUnrecordPrecodeAdded)
                keys.Add(null);

            return keys.Distinct();
        }

        public IEnumerable<QuotaCellInfo> GetQuotaCells(int surveySid, int quotaId, string[] fields,
           bool isSupportOptimisticQuota)
        {
            var selectFieldsSql =
                @"SELECT * 
                  FROM dbo.[BvSurveyQuotaCell]
                  WHERE [SurveyID] = @SurveyID AND [QuotaID] = @QuotaID";
            DataTable table = _databaseEngine.ExecuteDataTable<DataTable>(
                selectFieldsSql,
                CommandType.Text,
                new SqlParameter("@SurveyID", surveySid),
                new SqlParameter("@QuotaID", quotaId));
            foreach (DataRow row in table.Rows)
            {
                var fieldValues = XmlSerialization.Deserialize<QuotaCellData>((string)row["XmlData"]).FieldValues;
                var cell = new QuotaCellInfo
                {
                    Id = (int)row["CellID"],
                    Counter = (int)row["Counter"],
                    Limit = (int)row["Limit"],
                    LiveCounter = isSupportOptimisticQuota ? (int)row["LiveCounter"] : 0,
                    LiveLimit = isSupportOptimisticQuota ? (int)row["LiveLimit"] : (int)row["Limit"],
                    IsDisabled = (bool)row["IsDisabled"],
                    Key = fields.Select(x => fieldValues.FirstOrDefault(fv => fv.Field == x)?.Value).ToArray(),
                    IsOpen = (bool)row["IsOpen"],
                };

                // skip load cells with unrecords
                if (cell.KeyUnrecordMask != 0)
                    continue;

                yield return cell;
            }
        }
    }
}
