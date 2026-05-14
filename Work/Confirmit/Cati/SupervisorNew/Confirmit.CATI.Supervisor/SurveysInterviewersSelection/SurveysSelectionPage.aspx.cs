using System;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.Activity;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.SurveysInterviewersSelection
{
    public partial class SurveysSelectionPage : BaseForm
    {
        [StoreInViewState]
        public SourceList SourceList;

        [StoreInViewState]
        public int? SelectedSurveyId;

        private SurveyIdsSessionProvider _surveyIdsSessionProvider = new SurveyIdsSessionProvider();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                SourceList = (SourceList)Int32.Parse(Request.Params["SourceList"]);

                if (String.IsNullOrEmpty(Request.Params["SelectedId"]) == false)
                    SelectedSurveyId = Int32.Parse(Request.Params["SelectedId"]);
                
                InitSelectedSurveysIds();                
            }            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsSingleSelectionMode())
            {
                hint.Text = Strings.SelectSurvey;
                SetOverlayTitle("Select Survey");
            }

            doubleGrid.UseOnlyOpenSurveys = IsSourceActivityList();
            doubleGrid.SingleSelectionMode = IsSingleSelectionMode();
        }

        private void InitSelectedSurveysIds()
        {
            var ids = _surveyIdsSessionProvider.GetSelectedSurveys(SourceList, SelectedSurveyId);

            doubleGrid.SelectedSurveysIds = ids;
        }

        private bool IsSourceActivityList()
        {
            return (SourceList == SourceList.SurveysList || 
                    SourceList == SourceList.InterviewerList ||
                    SourceList == SourceList.AppointmentsList || 
                    SourceList == SourceList.PerformanceList);
        }

        private bool IsSingleSelectionMode()
        {
            return (SourceList == SourceList.SampleStatusSummary ||
                    SourceList == SourceList.NumberOfAttemptsReport ||
                    SourceList == SourceList.AttemptsByDespositionReport ||
                    SourceList == SourceList.InboundCallsReport ||
                    SourceList == SourceList.SampleUtilisationReport ||
                    SourceList == SourceList.QuotaProgressReport ||
                    SourceList == SourceList.SampleStatusSummaryByQuestionReport);
        }

        protected void SaveSelected(object sender, EventArgs e)
        {
            int[] ids = doubleGrid.SelectedSurveysIds;

            _surveyIdsSessionProvider.SetSelectedSurveyIds(SourceList, ids);            

            CloseOverlay(true);
        }
    }
}