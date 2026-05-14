using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.Repositories;

namespace Confirmit.CATI.Supervisor.Persons
{
    public partial class SelectAutomaticSurveyDialog : BaseForm
    {
        [StoreInViewState]
        protected int PersonId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["PersonId"] != null)
                    PersonId = Convert.ToInt32(Request["PersonId"]);
            }

            m_SurveyList.PersonId = PersonId;
        }

        protected void OKButtonClick(object sender, EventArgs e)
        {
            if (m_SurveyList.SelectedSurveyId.HasValue)
            {
                int id = m_SurveyList.SelectedSurveyId.Value;

                string formatedName = SurveyManager.FormatSurveyName(SurveyRepository.GetById(id));
                CloseOverlay(true, String.Format("{0},{1}", id, formatedName));
            }
            else
            {
                ShowClientMessage(Strings.Err_NoSurveyWasSpecified);
            }
        }
    }
}