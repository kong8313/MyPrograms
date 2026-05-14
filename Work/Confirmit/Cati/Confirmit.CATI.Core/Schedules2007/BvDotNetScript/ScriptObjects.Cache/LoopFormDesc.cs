using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public class LoopFormDesc : SingleFormDesc
    {
        public LoopFormDesc(int surveyId, string projectId, SingleForm form, SurveyDatabaseFormInfo dbFormInfo) : base(surveyId, projectId, form, dbFormInfo)
        {
        }
    }
}