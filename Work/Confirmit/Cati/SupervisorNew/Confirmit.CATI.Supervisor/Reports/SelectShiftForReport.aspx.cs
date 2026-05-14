using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;

namespace Confirmit.CATI.Supervisor.Reports
{
    public partial class SelectShiftForReport : BaseForm
    {
        private bool m_RangeChanged;

        [StoreInViewState]
        public SourceList SourceList;

        protected void Page_Init(object sender, EventArgs e)
        {

            if (IsPostBack == false)
            {
                SourceList = (SourceList) Int32.Parse(Request.Params["SourceList"]);
                if (SourceList == SourceList.CatiProductivityReport && ReportsSessionVariables.ShiftForInterviewerProductivityReport != null)
                {
                    dteShiftStartTime.DateTimeValueUtc = ReportsSessionVariables.ShiftForInterviewerProductivityReport.StartShiftTime;
                    dteShiftEndTime.DateTimeValueUtc = ReportsSessionVariables.ShiftForInterviewerProductivityReport.EndShiftTime;
                }

                if (SourceList == SourceList.SurveyOverviewReport && ReportsSessionVariables.ShiftForSurveyOverviewReport != null)
                {
                    dteShiftStartTime.DateTimeValueUtc = ReportsSessionVariables.ShiftForSurveyOverviewReport.StartShiftTime;
                    dteShiftEndTime.DateTimeValueUtc = ReportsSessionVariables.ShiftForSurveyOverviewReport.EndShiftTime;
                }

                if (SourceList == SourceList.ProductivityReport && ReportsSessionVariables.ShiftForSurveyProductivityReport != null)
                {
                    dteShiftStartTime.DateTimeValueUtc = ReportsSessionVariables.ShiftForSurveyProductivityReport.StartShiftTime;
                    dteShiftEndTime.DateTimeValueUtc = ReportsSessionVariables.ShiftForSurveyProductivityReport.EndShiftTime;
                }
            }

            dteShiftStartTime.ValueChanged +=
                delegate(object s, EventArgs ev)
                {
                        m_RangeChanged = true;

                };
            dteShiftEndTime.ValueChanged +=
                delegate(object s, EventArgs ev)
                {
                        m_RangeChanged = true;
                };
        }

        protected void SaveSelected(object sender, EventArgs e)
        {
            ShiftForReport shift=null;
            bool toUpdate = true;

            if (cbxResetShift.Checked)
                shift = null;
            else if (m_RangeChanged)
                shift = new ShiftForReport()
                {
                    StartShiftTime = dteShiftStartTime.DateTimeValueUtc,
                    EndShiftTime = dteShiftEndTime.DateTimeValueUtc
                };
            else
                toUpdate = false;  
          
            if (SourceList == SourceList.CatiProductivityReport && toUpdate)
                ReportsSessionVariables.ShiftForInterviewerProductivityReport = shift;
        
            if (SourceList == SourceList.SurveyOverviewReport && toUpdate)
                ReportsSessionVariables.ShiftForSurveyOverviewReport = shift;

            if (SourceList == SourceList.ProductivityReport && toUpdate)
                ReportsSessionVariables.ShiftForSurveyProductivityReport = shift;

            m_RangeChanged = false;
            CloseOverlay(true);
        }
    }
}