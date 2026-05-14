using System;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.Batch;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Core;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    public partial class MoveCalls : BaseActionForm
    {


        // Dialog size when user create Appointment
        private const int AppointmentPropertiesWidth = 540;
        private const int AppointmentPropertiestHeight = 420;

        private const int OriginalWidth = 500;
        private const int OriginalHeight = 235;


        /// <summary>
        /// Determins Move or MoveAndReschedule command executed
        /// </summary>
        private CallMoveType MoveType
        {
            get
            {
                return (CallMoveType)ViewState["MoveType"];
            }
            set
            {
                ViewState["MoveType"] = value;
            }
        }

        /// <summary>
        /// Count of calls in entire list.
        /// It is necessary to limit count of processed calls when we
        /// are going to process entire list.
        /// </summary>
        protected int EntireListItemsCount
        {
            get
            {
                return (int)ViewState["EntireListItemsCount"];
            }
            set
            {
                ViewState["EntireListItemsCount"] = value;
            }
        }

        /// <summary>
        /// Selected ITS ID
        /// </summary>
        private int SelectedIts
        {
            get
            {
                return int.Parse(ddlITS.SelectedItem.Value);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Buffer = true;

            if (!IsPostBack)
            {
                ResizeWindow(OriginalWidth, OriginalHeight);
                ddlITS.DataSource = SurveyService.GetTransientStates(SurveyID);
                ddlITS.DataValueField = "StateID";
                ddlITS.DataTextField = "Name";
                ddlITS.DataBind();
                ddlITS.Items.Insert(0, new ListItem(String.Empty, "0"));
                MoveType = (CallMoveType)Int32.Parse(Request.Params["MoveType"]);

                if (MoveType == CallMoveType.MoveAndReschedule &&
                    SelectionType == CallSelectionType.Filtered)
                {
                    EntireListItemsCount = Int32.Parse(Request.Params["EntireListItemsCount"]);
                }
            }

            if (MoveType == CallMoveType.MoveAndReschedule)
            {
                var limit = SurveyService.LimitCallsForMoveAndRescheduleAction;
                dialog.OKButton.OnClientClick =
                    String.Format(
                        "if (!confirmationPrcocessingLimit({0}, {1}, '{2}')) return;",
                        SelectionType == CallSelectionType.Selected ? IDS.Count : EntireListItemsCount,
                        limit,
                        String.Format(Strings.MoveAndRescheduleCallLimitConfirmation, limit));

                dialog.OKButton.Text = Strings.MoveAndReschedule;
            }
            else
            {
                dialog.OKButton.Text = Strings.Move;
            }
        }

        protected void SelectedStatusChanged(object sender, EventArgs e)
        {
            if (MoveType == CallMoveType.MoveAndReschedule && SelectedIts == (int) CallOutcome.Appointment)
            {
                bool isOneRecordSelected = (SelectionType == CallSelectionType.Selected &&
                                            ((SelectedBatchParameters) BatchParameters).Items.Length == 1)
                                           || (SelectionType == CallSelectionType.Filtered && EntireListItemsCount == 1);

                if (!isOneRecordSelected)
                {
                    ShowClientMessage(Strings.OneRecordShouldBeSelectedForAppointment);
                    ddlITS.SelectedIndex = 0;
                    return;
                }

                appointmentPropertiesPanel.Visible = true;

                ResizeWindow(AppointmentPropertiesWidth, AppointmentPropertiestHeight);
            }
            else
            {
                appointmentPropertiesPanel.Visible = false;
                ResizeWindow(OriginalWidth, OriginalHeight);
            }

        }

        /// <summary>
        /// Executing CallManager method for selected calls and selected ITS
        /// </summary>
        protected void OkButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (SelectedIts == 0)
                {
                    AddUserMessage(Strings.NoStatusSelected);
                    return;
                }

                if (MoveType == CallMoveType.MoveAndReschedule)
                {
                    LegacySupervisorMetrics.OnCallManagementAction("MoveAndReschedule");
                    BvAsyncOperationQueueEntity operationEntity = CallManager.MoveAndRescheduleCalls(SurveyID, SelectedIts, BatchParameters, 
                        SelectedIts == (int)CallOutcome.Appointment ?  AppointmentProperties.AppointmentData : null);

                    Redirect(operationEntity);
                }
                else if (MoveType == CallMoveType.Move)
                {
                    LegacySupervisorMetrics.OnCallManagementAction("Move");
                    BvAsyncOperationQueueEntity operationEntity = CallManager.MoveCalls(SurveyID, SelectedIts, BatchParameters);

                    Redirect(operationEntity);
                }                                                
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}
