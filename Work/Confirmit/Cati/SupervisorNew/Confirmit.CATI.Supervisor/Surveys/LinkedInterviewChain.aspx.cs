using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Confirmit.CATI.Core.DAL.Generated.Entity.Procedure;
using Confirmit.CATI.Core.DAL.Generated.Procedure.Adapter;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources;
using Infragistics.Web.UI.GridControls;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public partial class LinkedInterviewChain : BaseForm
    {
        [StoreInViewState]
        protected int? _linkedInterviewSessionId;

        [StoreInViewState]
        protected int _surveyId;

        [StoreInViewState]
        protected int _interviewId;

        protected void Page_Load(object sender, EventArgs e)
        {
            grid.GetPage =
                delegate(out int totalCount)
                {
                    var list = BvSpHistory_GetLinkedInterviewsAdapter.ExecuteEntityList(_linkedInterviewSessionId);

                    totalCount = list.Count();
                    return list;
                };

            if (!IsPostBack)
            {
                _surveyId = int.Parse(Request["SurveyId"]);
                _interviewId = int.Parse(Request["InterviewId"]);
                _linkedInterviewSessionId = int.Parse(Request["LinkedInterviewSessionId"]);
            }

            grid.GridName = String.Format(Strings.LinkedInterviews);
            grid.InitializeRow += InitializeHistoryRow;
        }

        private void InitializeHistoryRow(object sender, RowEventArgs e)
        {
            var historyItem = (BvSpHistory_GetLinkedInterviewsEntity)e.Row.DataItem;
            if (historyItem.SurveyId == _surveyId && historyItem.InterviewId == _interviewId)
            {
                e.Row.CssClass += " CurrentInterview";
            }
        }
    }
}