using System;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Core.Exceptions;
using Confirmit.CATI.Supervisor.Core.Surveys;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Supervisor.Classes.CallManagement;
using Confirmit.CATI.Core.Repositories;

using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Adapter;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Supervisor.Core.CallCenters;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.CallManagement
{
    /// <summary>
    /// Contains page for additon or editing calls.
    /// </summary>
    [CheckSurveyPermission(RequestParameterName = "ID")]
    public partial class CallProperties : BaseForm
    {

        private readonly ICallCenterProvider _callCenterProvider = ServiceLocator.Resolve<ICallCenterProvider>();
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ITimezoneService _timezoneService = ServiceLocator.Resolve<ITimezoneService>();
        private readonly IContextInfoService _contextInfoService = ServiceLocator.Resolve<IContextInfoService>();

        private BvCallEntity m_call;

        /// <summary>
        /// CallID passed in dialog params.
        /// </summary>
        private int? InterviewID
        {
            get { return (int?)ViewState["InterviewID"]; }
            set { ViewState["InterviewID"] = value; }
        }

        /// <summary>
        /// SurveyID passed in dialog params.
        /// </summary>
        protected Int32 SurveyID
        {
            get { return (int)ViewState["SurveyID"]; }
            set { ViewState["SurveyID"] = value; }
        }

        /// <summary>
        /// Time mode.
        /// </summary>
        protected ShowTimeMode ShowTimeMode
        {
            get
            {
                return (ShowTimeMode)ViewState["ShowTimeMode"];
            }
            set
            {
                ViewState["ShowTimeMode"] = value;
            }
        }

        /// <summary>
        /// Current Call instance.
        /// </summary>
        protected BvCallEntity CurrentCall
        {
            get
            {
                if (m_call == null)
                    m_call = CallQueueService.GetCallAndNoLock(SurveyID, InterviewID.Value);
                return m_call;
            }
        }

        private Int32 SelectedShiftType
        {
            get
            {
                return ddlShiftType.SelectedShiftTypeID;
            }
        }

        private Int32 SelectedInterviewID
        {
            get
            {
                return wneInterviewID.ValueInt;
            }
            set
            {
                wneInterviewID.Value = value;
            }
        }

        private bool Enable
        {
            get
            {
                return cbxEnable.Checked;
            }
            set
            {
                cbxEnable.Checked = value;
            }
        }

        private Int32 SelectedPriority
        {
            get
            {
                return wnePriority.ValueInt;
            }
            set
            {
                wnePriority.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the timezone ID of interview associated with the current call. Used in SelectedTimeToCall and SelectedTimeToExpire
        /// </summary>
        private int TimezoneID
        {
            get
            {
                switch (ShowTimeMode)
                {
                    case ShowTimeMode.Respondent:
                        return _timezoneService.GetTimezoneIdOrDefaultCallCenterTimezoneId(
                            InterviewRepository.GetById(SurveyID, SelectedInterviewID).TimezoneID);

                    default: return _timezoneProvider.GetLocalTimezoneId();
                }
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
                return cbxTimeToCall.Checked
                    ? (DateTime?)null
                    : TimezoneManager.ConvertToUTC(TimezoneID, dteTimeToCall.DateTimeValue);
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
        private DateTime SelectedTimeToExpire
        {
            get
            {
                return cbxTimeToExpire.Checked
                    ? CallHelper.FusionDateNever
                    : TimezoneManager.ConvertToUTC(TimezoneID, dteTimeToExpire.DateTimeValue);
            }
            set
            {
                if (value.Year >= 9999)
                {
                    cbxTimeToExpire.Checked = true;
                    dteTimeToExpire.DateTimeValue = TimezoneManager.ConvertToTzLocalTime(TimezoneID, DateTime.UtcNow);
                }
                else
                {
                    cbxTimeToExpire.Checked = false;
                    dteTimeToExpire.DateTimeValue = TimezoneManager.ConvertToTzLocalTime(TimezoneID, value);
                }
            }
        }

        #region Lifecycle.
        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveyID = int.Parse(Request.Params["ID"]);

                ddlShiftType.SurveyID = SurveyID;

                if (Request.Params["InterviewID"] != null)
                {
                    InterviewID = Int32.Parse(Request.Params["InterviewID"]);
                }

                if (Request.Params["ShowTimeMode"] != null)
                {
                    ShowTimeMode = (ShowTimeMode)Int32.Parse(Request.Params["ShowTimeMode"]);
                }
            }
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            BindControls();
            RegisterClientScripts();

            dialog.OKButton.Text = InterviewID.HasValue ? "Save" : "Create call";
        }

        #endregion

        #region Client scripts registration.
        private void RegisterClientScripts()
        {
            cbxTimeToCall.Attributes.Add("onclick", dteTimeToCall.ClientControllerName + ".setEnabled(!this.checked);");
            cbxTimeToExpire.Attributes.Add("onclick", dteTimeToExpire.ClientControllerName + ".setEnabled(!this.checked);");

            dteTimeToCall.Enabled = !cbxTimeToCall.Checked;
            dteTimeToExpire.Enabled = !cbxTimeToExpire.Checked;
        }
        #endregion.

        #region Methods.
        /// <summary>
        /// Binds data to controls regarding to selected call.
        /// </summary>
        private void BindControls()
        {
            if (InterviewID.HasValue)
            {
                SelectedInterviewID = CurrentCall.InterviewID;
                SelectedTimeToCall = CurrentCall.TimeInShift;
                SelectedTimeToExpire = CurrentCall.TimeToExpire.Value;
                Enable = CurrentCall.CallState != (int)CallState.DisabledByUser;
                wneInterviewID.Enabled = false;
                SelectedPriority = CurrentCall.Priority;
                ddlShiftType.SelectedShiftTypeID = CurrentCall.ShiftID;
            }
            else
            {
                Enable = true;
                dteTimeToCall.DateTimeValue = DateTime.Now;
                dteTimeToExpire.DateTimeValue = DateTime.Now;
            }
        }

        /// <summary>
        /// Handles the Click event of the OKButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OKButton_Click(object sender, EventArgs e)
        {
            BvCallEntity newCall;
            try
            {
                CheckTime();

                if (InterviewID.HasValue)
                {
                    // Edit mode 
                    newCall = CallQueueService.GetCallAndNoLock(SurveyID, SelectedInterviewID);
                }
                else
                {
                    // Create mode 
                    newCall = new BvCallEntity();
                    newCall.SurveySID = SurveyID;
                    newCall.InterviewID = SelectedInterviewID;
                }

                newCall.Priority = SelectedPriority;
                newCall.ShiftID = SelectedShiftType;
                newCall.TimeInShift = SelectedTimeToCall;
                newCall.TimeToExpire = SelectedTimeToExpire;
                newCall.CallState = Enable ? (int)CallState.Scheduled : (int)CallState.DisabledByUser;

                using (var transactionScope = new DatabaseTransactionScope("CallManagement.UpdateCall", DeadlockPriority.Supervisor))
                {
                    // Save changes
                    if (InterviewID.HasValue)
                    {
                        _contextInfoService.WriteContextInfo(0, OperationType.UpdateCall, _callCenterProvider.GetCurrentId(), 0);
                        CallManager.UpdateCall(newCall);
                    }
                    else
                    {
                        _contextInfoService.WriteContextInfo(0, OperationType.AddCall, _callCenterProvider.GetCurrentId(), 0);
                        CallManager.AddCall(newCall);
                    }

                    transactionScope.Commit();
                }

                CloseOverlay(true);
            }
            catch (InteviewNotExistsException ex)
            {
                AddUserMessage(ex);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        /// <summary>
        /// Checks  that time to expire isn't less than time to call.
        /// </summary>
        private void CheckTime()
        {
            if (cbxTimeToExpire.Checked == false && cbxTimeToCall.Checked == false)
            {
                DateTime timeToCall = dteTimeToCall.DateTimeValue;
                DateTime timeToExpire = dteTimeToExpire.DateTimeValue;
                if (timeToExpire < timeToCall)
                {
                    throw new UserMessageException(Strings.ExpTimeLessTimeToCall);
                }
            }
        }
        #endregion
    }
}