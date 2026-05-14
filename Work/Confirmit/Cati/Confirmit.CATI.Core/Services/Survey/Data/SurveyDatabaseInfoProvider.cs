using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Services.Survey.Data
{
    public class SurveyDatabaseInfoProvider : ISurveyDatabaseInfoProvider
    {
        private readonly ISurveyDatabaseEngine _surveyDatabaseEngine;

        public SurveyDatabaseInfoProvider(ISurveyDatabaseEngine surveyDatabaseEngine)
        {
            _surveyDatabaseEngine = surveyDatabaseEngine;
        }

        public SurveyDatabaseFormInfo GetFormInfo(int surveyId, string name)
        {
            var fields = GetFieldInfos(surveyId, name).Where(x => x.OtherType != OtherFieldType.IsOtherField).ToArray();

            var loopPath = GetLoopPath(surveyId, name);

            return new SurveyDatabaseFormInfo
            {
                Name = name,
                Fields = fields,
                LoopPath = loopPath
            };
        }

        private string[] GetLoopPath(int surveyId, string name)
        {
            var query = @"select lh.nesting_fieldname from <Schema>.form f 
	            left join <Schema>.form l on f.loopid = l.formid
	            left join <Schema>.loop_hierarchy lh on ISNULL( l.formname, 'responseid')  = lh.fieldname
	            where f.formname = @FormName
	            order by nesting_level";

            var loopPath = _surveyDatabaseEngine.ExecuteScalarList(surveyId, query, (r) =>
                (string)r["nesting_fieldname"],
                new SqlParameter("@FormName", name)
                ).ToArray();
            return loopPath;
        }

        private SurveyDatabaseFieldInfo[] GetFieldInfos(int surveyId, string name)
        {
            var query = @"SELECT field.fieldname AS fieldname, field.tableid AS tableid, ISNULL( field.other, 0 ) as other
		        FROM <Schema>.form INNER JOIN <Schema>.field on form.formid = field.parentid
		        WHERE form.formname = @FormName";

            var fields = _surveyDatabaseEngine.ExecuteScalarList(surveyId, query, (r) =>
                new SurveyDatabaseFieldInfo()
                {
                    FieldName = (string)r["fieldname"],
                    TableName = String.Format("response{0}", (int)r["tableid"]),
                    OtherType = (OtherFieldType)(int)r["other"]
                },
                new SqlParameter("@FormName", name)).ToArray();
            return fields;
        }


        public IEnumerable<SurveyDatabaseFieldInfo> GetRespondentFieldsInfo(int surveyId)
        {
            var query = @"SELECT * FROM <Schema>.[respondent] WHERE 1 <> 1";

            var table = _surveyDatabaseEngine.ExecuteQuery(surveyId, query);

            foreach (DataColumn column in table.Columns)
            {
                yield return new SurveyDatabaseFieldInfo()
                {
                    FieldName = column.ColumnName,
                    TableName = "respondent"
                };
            }
        }
    }
}
