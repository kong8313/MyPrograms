using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.WcfServices.Clients;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Confirmit.CATI.Supervisor.Core.Confirmit.QuotaViewExtension;

namespace Confirmit.CATI.Supervisor.Core.Surveys
{
    public class SurveyQuotasExportInfoProvider : ISurveyQuotasExportInfoProvider
    {

        private readonly IAuthoringService _authoringService;

        public SurveyQuotasExportInfoProvider(int surveyId, string surveyName)
        {
            SurveyId = surveyId;
            SurveyName = surveyName;
            _authoringService =  ServiceLocator.Resolve<IAuthoringService>();
        }

        public int SurveyId { get; private set; }

        public string SurveyName { get; private set; }

        public string[] GetQuotaNames()
        {
            return QuotaManager.GetQuotaNames(SurveyId);
        }

        public DataTable GetQuotaInfo(string quotaName)
        {
            int fieldsCount;
            var table = GetQuotasTable(quotaName, out fieldsCount);

            return GetQuotasTableWithJointFields(table, fieldsCount);
        }       

        private DataTable GetQuotasTable(string quotaName, out int fieldsCount)
        {
            var quotaList = QuotaManager.GetQuotaList(SurveyName, quotaName);

            var parameters = QuotaManager.GetExtraCounterParameters(ExtraQuotaCounterTypes.Scheduled, SurveyId, quotaList.QuotaId, false, null);

            var isQuotaBalancingEnabled = QuotaManager.GetBalancedQuotaNames(SurveyId).Contains(quotaName);

            var columnsBuilder = new AdditionalColumnsBuilderFactory().Create(quotaList.IsOptimistic, true, isQuotaBalancingEnabled, parameters);

            var fields = _authoringService.GetQuotaForms(SurveyName, quotaName).OfType<SingleForm>().ToList();            

            var table = QuotaManager.CreateQuotaDataTable(fields, columnsBuilder);

            QuotaManager.FillQuotaDataTable(table, quotaList, fields, columnsBuilder);

            fieldsCount = fields.Count;

            return table;
        }

        private DataTable GetQuotasTableWithJointFields(DataTable quotaTable, int fieldsCount)
        {
            var result = new DataTable();
            
            //remove ID column
            quotaTable.Columns.RemoveAt(0);

            var fieldsColumns = quotaTable.Columns.Cast<DataColumn>().Take(fieldsCount).ToArray();
            var otherColumns = quotaTable.Columns.Cast<DataColumn>().Skip(fieldsCount).ToArray();

            var jointFieldsColumnName = string.Join(", ", fieldsColumns.Select(x => x.ColumnName));
            var jointFieldsColumnCaption = string.Join(", ", fieldsColumns.Select(x => x.Caption));

            var jointFieldsColumn = new DataColumn(jointFieldsColumnName, typeof(string)) { Caption = jointFieldsColumnCaption };

            result.Columns.Add(jointFieldsColumn);

            foreach (var c in otherColumns)
                result.Columns.Add(new DataColumn(c.ColumnName, c.DataType) { Caption = c.Caption });
            
            foreach (DataRow row in quotaTable.Rows)
            {
                var newRow = result.NewRow();

                newRow[jointFieldsColumnName] = string.Join(", ", fieldsColumns.Select(x => row[x]).ToArray());

                foreach (var c in otherColumns)
                    newRow[c.ColumnName] = row[c.ColumnName];
            
                result.Rows.Add(newRow);
            }  
 
            return result;
        }
    }
}
