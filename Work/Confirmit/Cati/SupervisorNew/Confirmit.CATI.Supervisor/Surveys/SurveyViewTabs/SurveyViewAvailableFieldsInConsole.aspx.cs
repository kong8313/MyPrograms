using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class SurveyViewAvailableFieldsInConsole : BaseForm
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}