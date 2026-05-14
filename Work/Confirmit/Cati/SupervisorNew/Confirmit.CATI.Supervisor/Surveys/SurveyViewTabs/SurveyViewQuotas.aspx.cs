using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Core.SearchableFields;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class SurveyViewQuotas : BaseForm
    {
        public override string Title
        {
            get { return string.Format("{0}: {1}", Strings.QuotaPageTitle, SrvInfoQuotas.QuotaName); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public static string CallManagement(string cellfields, string values, int surveyId)
        {
            var selectedFields = cellfields.Split(',');
            var selectedValues = values.Split(',');
            var variablesToPass = new Dictionary<string,string>();

            for (var i = 0; i < selectedFields.Length; i++)
            {
                variablesToPass.Add(ConfirmitVariablesHelper.GetConfirmitVariableAlias(selectedFields[i]),selectedValues[i]);
            }
            
            var fields = new SearchableFieldsProvider().GetCallManagementSearchableFields(surveyId);
            var searchableFieldsService = new SearchableFieldsService();
            var existingFieldsIds = searchableFieldsService.GetBySurveyId(surveyId).Select(y => y.ColumnId).ToArray();

            var newColumns = fields.Where(x =>
                selectedFields.Contains(x.Name)
                && !existingFieldsIds.Contains(x.ColumnId)).ToArray();

            HttpContext.Current.Session[SessionVariablesLiterals.VariablesPassedFromQuotaToCallManagement] =
                variablesToPass;

            if (!newColumns.Any())
            {
                return "success";
            }

            var callManagementSettings = ServiceLocator.Resolve<ICallManagementSettings>();

            if (existingFieldsIds.Length + newColumns.Length > callManagementSettings.MaximumConfirmitVariables)
            {
                return string.Format(Strings.MaxSelectedRowsAlertConfirmitVariables,
                    callManagementSettings.MaximumConfirmitVariables,
                    existingFieldsIds.Length + newColumns.Length);
            }

            var evt = new SaveCallMangementSearchableFieldsEvent(
                    surveyId,
                    SurveyRepository.GetById(surveyId).Name,
                    newColumns.Select(x => x.Name).ToArray());

            using (var transactionScope = new DatabaseTransactionScope("UpdateCallManagementSearchField", DeadlockPriority.Supervisor))
            {
                foreach (var column in newColumns)
                {
                    searchableFieldsService.Add(surveyId, column.TableId, column.ColumnId);
                }

                transactionScope.Commit();
            }

            evt.Finish();

            return "success";
        }
    }
}
