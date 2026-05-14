using System;
using System.Linq;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Core.Transactions.Filters;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;

namespace Confirmit.CATI.Supervisor.Filter
{
    public partial class CopySurveySpecificFilters : BaseForm
    {
        [StoreInViewState]
        protected int SurveyId;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveyId = int.Parse(Request["ID"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            m_grid.HintText = "Select a survey from the list below to copy or move any survey specific filters from.";
            m_grid.GridName = SurveyService.GetFormattedSurveyName(SurveyId);

            m_grid.GetPage = delegate(out int totalCount)
            {
                var list = new FilterCopyingService().GetListOfSurveysToCopyFiltersFrom(SurveyId, User.Name);
                return BaseMethods.GetPage(list, m_grid.PageArguments, out totalCount);
            };
        }

        protected void moveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_grid.SelectedKeysInt.Count == 0)
                {
                    AddUserMessage("Please select a survey.");
                    return;
                }

                int sourceSurveyId = m_grid.SelectedKeysInt.First();

                new MoveSurveySpecificFiltersToSurveyTransaction(sourceSurveyId, SurveyId).Execute();

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void copyButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_grid.SelectedKeysInt.Count == 0)
                {
                    AddUserMessage("Please select a survey.");
                    return;
                }

                int sourceSurveyId = m_grid.SelectedKeysInt.First();

                new CopySurveySpecificFiltersToSurveyTransaction(sourceSurveyId, SurveyId).Execute();

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}