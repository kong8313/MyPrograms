using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public class OpenFormDesc : FormDescBase
    {
        public OpenFormDesc(int surveyId, string projectId, OpenForm form, SurveyDatabaseFormInfo dbFormInfo)
            : base(surveyId, projectId, form, dbFormInfo)
        {
            FillFormDescSpecificFields(form);

            if (OPEN)
                ValidationData = new OpenValidationData();
            else if (NUMERIC)
                ValidationData = new NumericValidationData(form.UpperLimitType, form.UpperLimit, form.LowerLimitType, form.LowerLimit);
            else if(DATE)
                ValidationData = new DateValidationData();
        }

        private void FillFormDescSpecificFields(OpenForm form)
        {
            if (form.Numeric)
            {
                NUMERIC = true;
            }
            else if (form.IsDate)
            {
                DATE = true;
            }
            else
                OPEN = true;
        }
    }
}