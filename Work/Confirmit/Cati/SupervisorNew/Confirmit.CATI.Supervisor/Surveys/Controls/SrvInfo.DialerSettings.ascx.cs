using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Supervisor.Resources;
using Confirmit.CATI.Supervisor.ServerControls;

using ConfirmitDialerInterface;

using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Supervisor.Surveys
{
    public partial class SrvInfoDialerSettings : SrvInfoChild
    {
        private readonly IDialerSurveyParametersManager _dialerSurveyParametersManager;
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        protected readonly IToggleSettings ToggleSettings;
        private bool needUpdate;

        public SrvInfoDialerSettings()
        {
            _dialerSurveyParametersManager = ServiceLocator.Resolve<IDialerSurveyParametersManager>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _inboundTelephoneNumberRepository = ServiceLocator.Resolve<IInboundTelephoneNumberRepository>();
            ToggleSettings = ServiceLocator.Resolve<IToggleSettings>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether current page loading is the first when PreRender is called. 
        /// </summary>
        /// <remarks>
        /// When this control is hidden - PreRender is not called,
        /// so we need to auto-bind data only when PreRender is called first time (control become visible).
        /// </remarks>
        private bool IsFirstLoad
        {
            get
            {
                if (ViewState["IsFirstLoad"] == null)
                {
                    ViewState["IsFirstLoad"] = true;
                }

                return (bool)ViewState["IsFirstLoad"];
            }

            set
            {
                ViewState["IsFirstLoad"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ParametersArea.ParametersCollection = _dialerSurveyParametersManager.GetDialerSurveyParameters(Survey.SID);
            var ddiNumbersInfo = string.Join(", ", _inboundTelephoneNumberRepository.GetBySurveyId(Survey.SID).Select(x => x.TelephoneNumber));
            lblDdiNumbersValues.Text = string.IsNullOrEmpty(ddiNumbersInfo) ? Strings.NotConfiguredDdiNumber : ddiNumbersInfo;
            toolbar.LeftLabel = string.Format(Strings.DialerSettingsForSurvey, Survey.Description, Survey.Name);

            DataBinding += SrvInfoDialerSettings_DataBinding;
        }

        protected void SrvInfoDialerSettings_DataBinding(object sender, EventArgs e)
        {
            ParametersArea.ParametersCollection = _dialerSurveyParametersManager.GetDialerSurveyParameters(Survey.SID);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsFirstLoad || needUpdate)
            {
                DataBind();
            }

            stateChecker.AddSaveButton(btnSave);
            IsFirstLoad = false;
        }

        /// <summary>
        /// Saves dialer parameters for the survey.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveDialerSettings(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateDialerParametersAndSetErrorMessages())
                {
                    return;
                }

                var evt = new SetDialerSurveyParametersEvent(Survey.SID, Survey.Name, ParametersArea.ParametersCollection);

                _supervisorServiceClient.SetDialerSurveyParameters(Survey.SID, ParametersArea.ParametersCollection);
                
                stateChecker.MarkAsUnchanged();

                evt.Finish();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        protected void ResetParams(object sender, EventArgs e)
        {
            try
            {
                var evt = new ResetDialerSurveyParametersEvent(Survey.SID, Survey.Name);
                _dialerSurveyParametersManager.ResetSurveyDialerParametersToDefaultValues(Survey.SID);
                evt.Finish();

                Server.TransferRequest(Request.Url.AbsolutePath, false);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
            finally
            {
                needUpdate = true;
            }
        }

        protected void Refresh(object sender, EventArgs e)
        {
            needUpdate = true;
        }

        /// <summary>
        /// In this method we perform both
        /// simple GUI validation and dialer validation.
        /// We should do dialer validation even if GUI validation failed
        /// because we need to show all error messages to the user.
        /// </summary>
        /// <returns>True if validation succeeded; false otherwise.</returns>
        private bool ValidateDialerParametersAndSetErrorMessages()
        {
            var guiValidationResult = ParametersArea.ValidateParametersAndSetErrorMessages();

            var dialerValidationResult = true;

            if (_dialerSurveyParametersManager.DoesDialerHaveSurveyParameters)
            {
                try
                {
                    _supervisorServiceClient.ValidateDialerSurveyParameters(ParametersArea.ParametersCollection);
                }
                catch (DialerParametersException ex)
                {
                    ProcessDialerParametersErrors(ex.Errors);
                    dialerValidationResult = false;
                }
                catch (Exception ex)
                {
                    Context.AddError(ex);
                }
            }

            return guiValidationResult && dialerValidationResult;
        }

        private void ProcessDialerParametersErrors(IEnumerable<DialerParameterError> errors)
        {
            foreach (var error in errors)
            {
                ParametersArea.SetError(error.Id, error.ErrorDescription);
            }
        }
    }
}