using System.Web;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls;
using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.Surveys.Controls
{
    /// <summary>
    /// Quotas list control.
    /// </summary>
    public partial class SrvInfoQuotas : SrvInfoChild
    {
        [StoreInViewState]
        protected BvSurveyEntity ActualSurvey;

        protected void Page_Load(object sender, EventArgs e)
        {
            var allQuotas = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["startAllQuotas"]);
            var enableAll = allQuotas != null && bool.Parse(allQuotas);

            if (ActualSurvey == null)
            {
                ActualSurvey = Survey;
            }

            if (AllQuotasBoard.SelectedSurveyId != 0 || SingleQuotaBoard.SelectedSurveyId != 0)
            {
                ActualSurvey =
                    SurveyRepository.GetById(AllQuotasBoard.SelectedSurveyId != 0
                        ? AllQuotasBoard.SelectedSurveyId
                        : SingleQuotaBoard.SelectedSurveyId);
                SingleQuotaBoard.RefreshGrid();
            }

            bool isAllQuotasEnabled = AllQuotasBoard.Visible;
            SingleQuotaBoard.Survey = ActualSurvey;
            AllQuotasBoard.Survey = ActualSurvey;

            if (IsPostBack)
            {
                if (SingleQuotaBoard.QuotaName == Strings.AllQuotas && SingleQuotaBoard.Visible)
                {
                    isAllQuotasEnabled = true;
                }

                if (AllQuotasBoard.QuotaName != Strings.AllQuotas && AllQuotasBoard.Visible)
                {
                    isAllQuotasEnabled = false;
                    SingleQuotaBoard.SetQuota(AllQuotasBoard.QuotaName);
                }
            }
            else
            {
                isAllQuotasEnabled = enableAll;
            }

            AllQuotasBoard.Visible = isAllQuotasEnabled;
            SingleQuotaBoard.Visible = !isAllQuotasEnabled;
        }

        public string QuotaName
        {
            get { return SingleQuotaBoard.QuotaName; }
        }
    }
}
