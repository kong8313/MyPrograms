using System;
using System.Collections.Generic;
using System.Linq;
using Confirmit.CATI.Common.Validators;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Supervisor.Classes;

using ConfirmitDialerInterface;

using DialerCommon.DialerParameters;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DialerSettings : BaseForm
    {
        private readonly IDialerSurveyParametersManager _dialerSurveyParametersManager;
        private readonly IInputParameterValidator _inputParameterValidator;
        private readonly IEmailSettings _emailSettings;
        private readonly IDialerSettings _dialerSettings;
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly ISupervisorServiceClient _supervisorServiceClient;

        public DialerSettings()
        {
            _dialerSurveyParametersManager = ServiceLocator.Resolve<IDialerSurveyParametersManager>();
            _inputParameterValidator = ServiceLocator.Resolve<IInputParameterValidator>();
            _emailSettings = ServiceLocator.Resolve<IEmailSettings>();
            _dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
            _emailNotificationService = ServiceLocator.Resolve<IEmailNotificationService>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            EmailParameter.SourceParameter = new DialerParameter
            {
                Id = "Email",
                Name = Strings.EmailAddressForErrorNotification,
                Type = typeof(string).ToString(),
                Value = _emailSettings.NotificationEmailRecipients
            };
            
            RespondentVariablesParameter.SourceParameter = new DialerParameter
            {
                Id = "RespondentVariableToSendToTheDialer",
                Name = Strings.RespondentVariableToSendToTheDialer,
                Type = typeof(string).ToString(),
                Value = _dialerSettings.RespondentVariablesToSend
            };

            var parameters = _dialerSurveyParametersManager.GetDialerDefaultSurveyParameters();
            ParametersArea.ParametersCollection = parameters;
            ParametersArea.Visible = ParametersHint.Visible =
                _dialerSurveyParametersManager.DoesDialerHaveSurveyParameters && parameters != null;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                DataBind();
            }

            stateChecker.AddSaveButton(btnSave);
        }

        /// <summary>
        /// Saves dialer settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveDialerSettings(object sender, EventArgs e)
        {
            try
            {
                bool paramsValidateResult = ValidateDialerParametersAndSetErrorMessages();

                bool emailValidateResult = ValidateEmailParameterAndSetErrorMessage();

                if (paramsValidateResult && emailValidateResult)
                {
                    SaveDialerParameters();
                    SaveEmailParameter();
                    SaveRespondentVariablesParameter();
                }

                stateChecker.MarkAsUnchanged();
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
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
            bool guiValidationResult = ParametersArea.ValidateParametersAndSetErrorMessages();

            bool dialerValidationResult = true;
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

        private bool ValidateEmailParameterAndSetErrorMessage()
        {
            var emails = _emailNotificationService.ParseEmailString(EmailParameter.SourceParameter.Value);

            if (emails.Any(email => _inputParameterValidator.IsValidEmail(email) == false))
            {
                EmailParameter.ErrorMessage = Strings.EmailInvalidFormatMessage;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Methods displays error message for each incorrect parameter
        /// </summary>
        /// <param name="errors"></param>
        private void ProcessDialerParametersErrors(IEnumerable<DialerParameterError> errors)
        {
            foreach (var error in errors)
            {
                ParametersArea.SetError(error.Id, error.ErrorDescription);
            }
        }

        private void SaveEmailParameter()
        {
            var email = EmailParameter.SourceParameter.Value;
            
            var evt = new SetDialerNotificationsEmailEvent(email);

            _emailSettings.NotificationEmailRecipients = email;

            evt.Finish();
        }
        
        private void SaveRespondentVariablesParameter()
        {
            var variables = RespondentVariablesParameter.SourceParameter.Value;

            if (variables == _dialerSettings.RespondentVariablesToSend)
                return;
            
            var evt = new SetRespondentVariablesToSendToTheDialerEvent(variables);

            _dialerSettings.RespondentVariablesToSend = variables;

            evt.Finish();
        }

        private void SaveDialerParameters()
        {
            if (_dialerSurveyParametersManager.DoesDialerHaveSurveyParameters)
            {
                var evt = new SetDialerDefaultSurveyParametersEvent(ParametersArea.ParametersCollection);
                _dialerSurveyParametersManager.SetDialerDefaultSurveyParameters(
                    ParametersArea.ParametersCollection);
                evt.Finish();
            }
        }
    }
}