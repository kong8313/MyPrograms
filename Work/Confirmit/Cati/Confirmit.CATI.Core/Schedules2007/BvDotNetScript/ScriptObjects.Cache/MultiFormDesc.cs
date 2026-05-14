using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public class MultiFormDesc : FormDescBase
    {
        public MultiFormDesc(int surveyId, string projectId, MultiForm form, SurveyDatabaseFormInfo dbFormInfo)
            : base(surveyId, projectId, form, dbFormInfo)
        {
            FillFormDescSpecificFields(form);

            if (OPEN)
                ValidationData = new OpenValidationData();
            else if (NUMERIC)
                ValidationData = new NumericValidationData(form.UpperLimitType, form.UpperLimit, form.LowerLimitType, form.LowerLimit);
        }

        private void FillFormDescSpecificFields(MultiForm form)
        {
            if (form.Numeric)
            {
                NUMERIC = true;
            }
            else
                OPEN = true;

            foreach (var answer in form.MultiAnswers.Items)
            {
                if (answer is HeaderAnswerEnd ||
                    answer is HeaderAnswer )
                {
                    continue;
                }

                if (answer.Precode != null)
                {
                    Categories.Add(answer.Precode);
                }
            }
        }
    }
}