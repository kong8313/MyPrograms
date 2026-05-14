using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Surveys;

namespace Confirmit.CATI.Supervisor.CallManagement.Controls
{
    public partial class SelectSurvey : BaseForm
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            surveyListGrid.GetPage = GetPage;
        }

        protected void SelectSurveyButtonClick(object sender, EventArgs e)
        {
            try
            {
                List<int> selectedSurveys = surveyListGrid.SelectedKeysInt;
                if (selectedSurveys.Count == 0)
                {
                    CloseOverlay();
                    return;
                }

                int selectedSurveyId = surveyListGrid.SelectedKeysInt.First();
                CallManager.AttachSurveyDbBySurveyId(selectedSurveyId);

                CloseOverlay(true, selectedSurveyId.ToString());
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected object GetPage(out int totalCount)
        {
            List<SurveyInfoItem> list = cbRecent.Checked ?
                    SurveyManager.GetRecentSurveysDescending(User.Name, string.Empty) :
                    SurveyManager.GetSurveys(User.Name, string.Empty);

            return BaseMethods.GetPage(list, surveyListGrid.PageArguments, out totalCount);
        }
    }
}
