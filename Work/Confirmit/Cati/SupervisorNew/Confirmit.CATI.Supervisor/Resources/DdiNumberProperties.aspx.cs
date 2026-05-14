using System;
using System.Drawing;
using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DdiNumberProperties : BaseForm
    {
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        private readonly ISupervisorServiceClient _supervisorServiceClient;
        private readonly IDialersRepository _dialersRepository;
        private readonly ISurveyRepository _surveyRepository;

        [StoreInViewState]
        protected string TelephoneNumber;

        [StoreInViewState]
        protected int? OldDialerId;

        [StoreInViewState]
        protected string SaveAudioMessagesJson;

        protected bool IsNew
        {
            get { return string.IsNullOrEmpty(TelephoneNumber); }
        }

        public DdiNumberProperties()
        {
            _inboundTelephoneNumberRepository = ServiceLocator.Resolve<IInboundTelephoneNumberRepository>();
            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _surveyRepository = ServiceLocator.Resolve<ISurveyRepository>();
        }

        // Prevent an unhandled exception when we select survey with empty telephone number
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
            }
            catch (UserMessageException ex)
            {
                System.Diagnostics.Trace.TraceWarning(ex.ToString());
            }
        }

        private bool IsSurveyDeleted(int? surveyId, out string surveyName)
        {
            if (surveyId == null)
            {
                surveyName = null;
                return true;
            }

            var survey = _surveyRepository.GetById(surveyId.Value);
            surveyName = survey.Name;
            if (survey.State == (int)SurveyState.SoftDeleted)
            {
                return true;
            }

            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            dialog.CancelButton.Attributes["onclick"] = "overlay.closeLast()";

            if (!IsPostBack)
            {
                ddlDialers.DataTextField = "Name";
                ddlDialers.DataValueField = "Id";
                ddlDialers.DataSource = _dialersRepository.GetAll();
                ddlDialers.DataBind();

                TelephoneNumber = Request["TelephoneNumber"];

                if (!IsNew)
                {
                    var ddiNumber = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(TelephoneNumber);
                    if (ddiNumber != null)
                    {
                        lblTelephoneNumberValue.Text = tbTelephoneNumber.Text = TelephoneNumber;
                        if (IsSurveyDeleted(ddiNumber.SurveyId, out var surveyName))
                        {
                            lblSurveyName.Text = Strings.SurveyIsDeleted;
                            lblSurveyName.ForeColor = Color.Red;
                        }
                        else
                        {
                            lblSurveyName.Text = surveyName;
                            selectedSurveyId.Value = ddiNumber.SurveyId.ToString();
                        }
                        
                        if (_dialersRepository.GetById(ddiNumber.DialerId) != null)
                        {
                            OldDialerId = ddiNumber.DialerId;
                            ddlDialers.SelectedValue = ddiNumber.DialerId.ToString();
                        }
                        
                        SaveAudioMessagesJson = ddiNumber.AudioMessagesJson;
                    }
                }

                dialog.OKButton.Text = IsNew ? Strings.Add : Strings.Save;
            }
            else 
            {
                if (int.TryParse(selectedSurveyId.Value, out var surveyId))
                {
                    var survey = _surveyRepository.TryGetById(surveyId);
                    lblSurveyName.Text = survey != null ? survey.Name : Strings.UnknownSurvey;
                    lblSurveyName.ForeColor = Color.Black;
                }
            }
        }

        private bool _closeOverlay;

        protected void OKButtonClick(object sender, EventArgs e)
        {
            _closeOverlay = true;
            try
            {
                if (string.IsNullOrEmpty(selectedSurveyId.Value) || !int.TryParse(selectedSurveyId.Value, out var surveyId))
                {
                    AddUserMessage(Strings.PleaseSelectOneSurvey);
                    return;
                }

                if (!int.TryParse(ddlDialers.SelectedValue, out var dialerId))
                {
                    AddUserMessage(Strings.PleaseSelectOneDialer);
                    return;
                }

                if (IsNew)
                {
                    var existingNumbers = _inboundTelephoneNumberRepository.TryGetByTelephoneNumber(tbTelephoneNumber.Text);
                    if (existingNumbers == null)
                    {
                        InsertEntity(tbTelephoneNumber.Text, surveyId, dialerId);
                    }
                    else
                    {
                        AddUserMessage(Strings.Err_DuplicateTelephoneNumber);
                        return;
                    }
                }
                else
                {
                    UpdateEntity(surveyId, dialerId);
                }

                if (_closeOverlay)
                {
                    CloseOverlay(true, null, true);
                }
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private void InsertEntity(string telephoneNumber, int surveyId, int dialerId)
        {
                var evt = new AddDdiNumberEvent(telephoneNumber);

                _inboundTelephoneNumberRepository.Insert(new BvInboundTelephoneNumberEntity
                {
                    TelephoneNumber = telephoneNumber,
                    SurveyId = surveyId,
                    DialerId = dialerId
                });

                ReConfigureDialerWithDdiNumbers(dialerId, Strings.WarningDuringDdiNumberAddition);
                evt.Finish();
        }

        private void UpdateEntity(int surveyId, int dialerId)
        {
            var evt = new UpdateDdiNumberEvent(surveyId, TelephoneNumber);

            _inboundTelephoneNumberRepository.Update(new BvInboundTelephoneNumberEntity
            {
                TelephoneNumber = TelephoneNumber,
                SurveyId = surveyId,
                DialerId = dialerId,
                AudioMessagesJson = SaveAudioMessagesJson
            });

            if (OldDialerId.HasValue && OldDialerId.Value != dialerId)
            {
                ReConfigureDialerWithDdiNumbers(OldDialerId.Value, string.Format(Strings.WarningDuringDdiNumberEditing, _dialersRepository.GetById(OldDialerId.Value).Name));
                ReConfigureDialerWithDdiNumbers(dialerId, string.Format(Strings.WarningDuringDdiNumberEditing, _dialersRepository.GetById(dialerId).Name));
            }
            else if (!OldDialerId.HasValue)
            {
                ReConfigureDialerWithDdiNumbers(dialerId, string.Format(Strings.WarningDuringDdiNumberEditing, _dialersRepository.GetById(dialerId).Name));
            }

            evt.Finish();
        }

        private void ReConfigureDialerWithDdiNumbers(int dialerId, string errorMessage)
        {
            try
            { 
                _supervisorServiceClient.ConfigureInboundDdiNumbers(dialerId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceWarning(ex.ToString());
                _closeOverlay = false;
                RegisterStartupScript($"showMessageAndCloseFrame('{errorMessage} {ex.Message}')");
            }
}
    }
}