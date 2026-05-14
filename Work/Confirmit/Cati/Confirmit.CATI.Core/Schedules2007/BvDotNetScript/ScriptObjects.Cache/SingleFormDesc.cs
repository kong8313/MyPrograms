using System.Collections.Generic;
using BvDotNetScript.ScriptObjects.Cache;
using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache.FormDescValidators;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Data;

namespace Confirmit.CATI.Core.Schedules2007.BvDotNetScript.ScriptObjects.Cache
{
    public class SingleFormDesc : FormDescBase
    {
        public SingleFormDesc(int surveyId, string projectId, SingleForm form, SurveyDatabaseFormInfo dbFormInfo)
            : base(surveyId, projectId, form, dbFormInfo)
        {
            FillFormDescSpecificFields(form);

            var preCodes = new Dictionary<string, string>();
            foreach (var answer in form.SingleAnswers.Items)
            {
                if (answer is HeaderAnswerEnd ||
                    answer is HeaderAnswer )
                {
                    continue;
                }

                if (answer.Precode != null)
                {
                    preCodes.Add(answer.Precode, null);
                }
            }

            ValidationData = new SingleValidationData(preCodes);
        }

        private void FillFormDescSpecificFields(SingleForm form)
        {
            CODED = true;
            if (form.IsBoolean)
                BOOL = true;
        }
    }
}