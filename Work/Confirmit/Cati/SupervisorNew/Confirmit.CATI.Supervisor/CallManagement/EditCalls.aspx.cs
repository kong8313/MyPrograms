using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Core;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    /// <summary>
    /// Contains page for addition or editing calls.
    /// </summary>
    public partial class EditCalls : BaseActionForm
    {
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ITimezoneService _timezoneService = ServiceLocator.Resolve<ITimezoneService>();

        private BvSurveyEntity _survey;

        private bool IsHybridDialingSupported()
        {
            if (_survey == null)
            {
                _survey = SurveyRepository.GetById(SurveyID);
            }

            return _survey.DialingMode == DialingMode.Predictive || _survey.DialingMode == DialingMode.Automatic;
        }

        private int? InterviewId
        {
            get
            {
                if (IDS.Count == 1)
                {
                    return IDS[0];
                }

                return null;
            }
        }

        private int? SelectedShiftType
        {
            get => shiftTypeNameToggle.Enabled ? ddlShiftType.SelectedShiftTypeID : (int?)null;
            set => ddlShiftType.SelectedShiftTypeID = value ?? 0;
        }

        private bool? SelectedCallState
        {
            get => callStateToggle.Enabled ? Convert.ToBoolean(callStateSelectedValue.Value) : (bool?)null;
            set => callStateSelectedValue.Value = value.ToString();
        }

        private int? SelectedPriority
        {
            get => callPriorityToggle.Enabled ? wnePriority.ValueInt : (int?)null;
            set => wnePriority.Value = value;
        }

        private int? SelectedExtendedStatus
        {
            get => extendedStatusToggle.Enabled ? int.Parse(ddlExtendedStatus.SelectedItem.Value) : (int?)null;
            set => ddlExtendedStatus.SelectedValue = value.ToString();
        }

        private byte? SelectedDialingMode
        {
            get
            {
                if (!dialingModeToggle.Enabled)
                {
                    return null;
                }

                switch (dialingModeSelectedValue.Value)
                {
                    case "Preview":
                        return (int)DialingMode.Preview;
                    case "Special":
                        return (int)DialingMode.SpecialDial;
                    default:
                        return 0;
                }
            }

            set
            {
                if (!value.HasValue)
                {
                    value = 0;
                }

                switch (value.Value)
                {
                    case (int)DialingMode.Preview:
                        dialingModeSelectedValue.Value = "Preview";
                        break;
                    case (int)DialingMode.SpecialDial:
                        dialingModeSelectedValue.Value = "Special";
                        break;
                    default:
                        dialingModeSelectedValue.Value = "Default";
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the timezone ID of interview associated with the current call. Used in SelectedTimeToCall and SelectedTimeToExpire
        /// </summary>
        private int TimezoneID
        {
            get
            {
                if (!InterviewId.HasValue)
                {
                    return _timezoneProvider.GetLocalTimezoneId();
                }

                return _timezoneService.GetTimezoneIdOrDefaultCallCenterTimezoneId(InterviewRepository.GetById(SurveyID, InterviewId.Value).TimezoneID);
            }
        }

        /// <summary>
        /// Sets time to call to DateTimeEditor and checkbox if needed according to DateTime value passed in.
        /// Gets time to call selected in DateTimeEditor or 30.12.1899 according to the checkbox state.
        /// </summary>
        private DateTime? SelectedTimeToCall
        {
            get
            {
                if (!timeToCallToggle.Enabled)
                {
                    return null;
                }

                return cbxTimeToCall.Checked
                    ? DateTime.MinValue
                    : dteTimeToCall.DateTimeValue;
            }

            set
            {
                if (value.HasValue)
                {
                    if (value.Value.Year <= 1900)
                    {
                        cbxTimeToCall.Checked = true;
                        dteTimeToCall.DateTimeValue = TimezoneManager.ConvertToTzLocalTime(TimezoneID, DateTime.UtcNow);
                    }
                    else
                    {
                        cbxTimeToCall.Checked = false;
                        dteTimeToCall.DateTimeValue = TimezoneManager.ConvertToTzLocalTime(TimezoneID, value.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Sets time to expire to DateTimeEditor and checkbox if needed according to DateTime value passed in.
        /// Gets time to expire selected in DateTimeEditor or 01.01.9999 according to the checkbox state.
        /// </summary>
        private DateTime? SelectedTimeToExpire
        {
            get
            {
                if (!timeToExpireToggle.Enabled)
                {
                    return null;
                }

                return cbxTimeToExpire.Checked
                    ? CallHelper.FusionDateNever
                    : dteTimeToExpire.DateTimeValue;
            }

            set
            {
                if (!value.HasValue || value.Value.Year >= 9999)
                {
                    cbxTimeToExpire.Checked = true;
                    dteTimeToExpire.DateTimeValue = TimezoneManager.ConvertToTzLocalTime(TimezoneID, DateTime.UtcNow);
                }
                else
                {
                    cbxTimeToExpire.Checked = false;
                    dteTimeToExpire.DateTimeValue = TimezoneManager.ConvertToTzLocalTime(TimezoneID, value.Value);
                }
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlShiftType.SurveyID = SurveyID;

                ddlExtendedStatus.DataSource = SurveyService.GetTransientStates(SurveyID);
                ddlExtendedStatus.DataValueField = "StateID";
                ddlExtendedStatus.DataTextField = "Name";
                ddlExtendedStatus.DataBind();
            }
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindControls();
                RegisterClientScripts();
                SetTitleAndGeneralInfo();

                dialog.OKButton.Text = Strings.Save;
            }

            dteTimeToCall.Enabled = timeToCallToggle.Enabled && !cbxTimeToCall.Checked;
            dteTimeToExpire.Enabled = timeToExpireToggle.Enabled && !cbxTimeToExpire.Checked;
        }

        private void RegisterClientScripts()
        {
            if (!InterviewId.HasValue)
            {
                // Note: this action has been done to prevent problem with getting Checked state from checkboxes
                // if set Enabled=false on the client side as for other controls
                ClientScript.RegisterStartupScript(GetType(), "OnLoad", "disableCheckBoxes();", true);
            }

            cbxTimeToCall.Attributes.Add("onclick", dteTimeToCall.ClientControllerName + ".setEnabled(!this.checked);");
            cbxTimeToExpire.Attributes.Add("onclick", dteTimeToExpire.ClientControllerName + ".setEnabled(!this.checked);");

            dteTimeToCall.Enabled = !cbxTimeToCall.Checked;
            dteTimeToExpire.Enabled = !cbxTimeToExpire.Checked;
        }

        /// <summary>
        /// Binds data to controls regarding to selected call.
        /// </summary>
        private void BindControls()
        {
            if (!IsHybridDialingSupported())
            {
                dialingModeInfo.Visible = false;
            }

            if (InterviewId.HasValue)
            {
                timeToCallToggle.Enabled =
                timeToExpireToggle.Enabled =
                callPriorityToggle.Enabled =
                callStateToggle.Enabled = 
                shiftTypeNameToggle.Enabled = 
                extendedStatusToggle.Enabled = true;

                if (IsHybridDialingSupported())
                {
                    dialingModeToggle.Enabled = true;
                }

                var currentCall = CallQueueService.GetCallAndNoLock(SurveyID, InterviewId.Value);
                var currentInterview = InterviewRepository.GetById(SurveyID, InterviewId.Value);

                SelectedTimeToCall = currentCall.TimeInShift;
                SelectedTimeToExpire = currentCall.TimeToExpire ?? DateTime.Now;
                SelectedCallState = currentCall.CallState != (int)Common.CallState.DisabledByUser;
                SelectedPriority = currentCall.Priority;
                SelectedShiftType = currentCall.ShiftID;
                SelectedExtendedStatus = currentInterview.TransientState;
                SelectedDialingMode = currentInterview.DialingMode;
            }
            else
            {
                SelectedCallState = true;
                dteTimeToCall.DateTimeValue = DateTime.Now;
                dteTimeToExpire.DateTimeValue = DateTime.Now;
                SelectedDialingMode = 0;
            }
        }

        private void SetTitleAndGeneralInfo()
        {
            lblInfo.Text = Strings.EditCallsGeneralInfo;

            if (IDS.Count == 1)
            {
                SetOverlayTitle(Strings.EditCallPropertiesOfOneItem);
                lblInfo.Text = Strings.EditCallsGeneralInfoForOneCall;
            }
            else if (IDS.Count > 1)
            {
                SetOverlayTitle(string.Format(Strings.EditCallPropertiesOfSelectedItems, IDS.Count));
            }
            else
            {
                SetOverlayTitle(Strings.EditCallPropertiesOfEntireList);
            }
        }
        
        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            try
            {
                int? callState = null;
                if (SelectedCallState.HasValue)
                {
                    callState = SelectedCallState.Value ? (int)Common.CallState.Scheduled : (int)Common.CallState.DisabledByUser;
                }

                if (!SelectedTimeToCall.HasValue && !SelectedTimeToExpire.HasValue && !callState.HasValue && !SelectedPriority.HasValue &&
                    !SelectedShiftType.HasValue && !SelectedExtendedStatus.HasValue && !SelectedDialingMode.HasValue)
                {
                    AddUserMessage(Strings.EditCallsNoSelectedToggles);
                    return;
                }

                if (SelectedTimeToCall.HasValue && SelectedTimeToExpire.HasValue && SelectedTimeToCall.Value > SelectedTimeToExpire.Value)
                {
                    AddUserMessage(Strings.EditCallsTimeToCallIsBiggerThenTimeToExpire);
                    return;
                }

                LegacySupervisorMetrics.OnCallManagementAction("Edit");
                var operationEntity = CallManager.EditCalls(
                    SurveyID,
                    SelectedTimeToCall,
                    SelectedTimeToExpire,
                    callState,
                    SelectedPriority,
                    SelectedShiftType,
                    SelectedExtendedStatus,
                    SelectedDialingMode,
                    BatchParameters);

                Redirect(operationEntity);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }
}