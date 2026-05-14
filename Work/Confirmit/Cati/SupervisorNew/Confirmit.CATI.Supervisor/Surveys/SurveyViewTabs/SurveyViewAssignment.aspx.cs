using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Surveys.SurveyViewTabs
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class SurveyViewAssignment : BaseForm
    {
    }
}