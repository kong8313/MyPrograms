using System;
using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Supervisor.Classes;

using Infragistics.Web.UI.GridControls;

using Strings = Confirmit.CATI.Supervisor.Resources.Strings;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class CallsPromotionHistory : SurveyFormBase
    {
        [StoreInViewState]
        protected int SurveyId;

        public override string Title
        {
            get { return Strings.CallsPromotionHistory; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveyId = int.Parse(Request["ID"]);
            }

            dtrsDates.RangeIntervals = 
                DateTimeRange.Last2Hrs | 
                DateTimeRange.Last4Hrs | 
                DateTimeRange.Today | 
                DateTimeRange.TodayMinus1 | 
                DateTimeRange.Last2Days | 
                DateTimeRange.ThisWeek;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            grid.GetPage = delegate(out int totalCount)
                {
                    var list = PromotionHistoryRepository.GetPromotionHistory(SurveyId, dtrsDates.BeginDateTime, dtrsDates.EndDateTime);

                    totalCount = list.Count();
                    return list;
                };

            BvSurveyEntity survey = SurveyRepository.GetById(SurveyId);
            dialog.Title = String.Format(Strings.QuotaPromotionHistoryHeader, survey.Description, survey.Name);
            grid.InitializeRow += Grid_InitializeRow;
        }

        private void Grid_InitializeRow(object sender, RowEventArgs e)
        {
            int callsToPromoteCount = Convert.ToInt32(e.Row.Items.FindItemByKey("CallsToPromoteCount").Value);
            int promotedCallsCount = Convert.ToInt32(e.Row.Items.FindItemByKey("PromotedCallsCount").Value);

            if (promotedCallsCount < callsToPromoteCount)
            {
                e.Row.CssClass += " Alert";
            }
        }
    }
}

