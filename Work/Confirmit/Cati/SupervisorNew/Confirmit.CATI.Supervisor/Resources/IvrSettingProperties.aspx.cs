using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.ConfigurationsApi;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class IvrSettingProperties : BaseForm
    {
        private readonly IIvrSettingsRepository _ivrSettingsRepository;
        private readonly IConfigurationApiService _configurationApiService;

        [StoreInViewState]
        protected string CurrentLanguageId;

        protected bool IsNew => string.IsNullOrEmpty(CurrentLanguageId);

        public IvrSettingProperties()
        {
            _ivrSettingsRepository = ServiceLocator.Resolve<IIvrSettingsRepository>();
            _configurationApiService = ServiceLocator.Resolve<IConfigurationApiService>();
        }

        private string GetLanguageDescription(LanguageModel language)
        {
            return $"{language.Name}, {language.CombinedId} ({language.Id})";
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            if (!IsPostBack)
            {
                List<LanguageModel>  languages = _configurationApiService.GetLanguages();

                ddlLanguages.Items.Clear();
                foreach (var language in languages)
                {
                    ddlLanguages.Items.Add(new ListItem(GetLanguageDescription(language), language.Id));
                }

                CurrentLanguageId = Request["LanguageId"];

                if (!IsNew)
                {
                    var language = _ivrSettingsRepository.TryGetByLanguageId(Convert.ToInt32(CurrentLanguageId));

                    if (language == null)
                    {
                        AddUserMessage(Strings.InternalServerError);
                        CloseOverlay(true);
                    }

                    ddlLanguages.SelectedValue = language.LanguageId.ToString();
                    tbWrongInputAudioUrl.Text = language.WrongInputAudioUrl;
                    tbWrongInputText.Text = language.WrongInputText;
                    tbWrongInputExitAudioUrl.Text = language.WrongInputExitAudioUrl;
                    tbWrongInputExitText.Text = language.WrongInputExitText;
                }
                else
                {
                    ddlLanguages.SelectedValue = "9";
                }

                dialog.OKButton.Text = IsNew ? Strings.Add : Strings.Save;
            }
        }

        protected void OkButtonClick(object sender, EventArgs e)
        {
            try
            {
                int selectedLanguageId = Convert.ToInt32(ddlLanguages.SelectedValue);

                if (!ValidateParameters(selectedLanguageId))
                {
                    return;
                }                

                var ivrSettings = new BvIvrSettingsEntity
                {
                    LanguageId = selectedLanguageId,
                    LanguageDescription = ddlLanguages.SelectedItem.Text.Substring(0, ddlLanguages.SelectedItem.Text.LastIndexOf(" (")),
                    WrongInputAudioUrl = tbWrongInputAudioUrl.Text,
                    WrongInputText = tbWrongInputText.Text,
                    WrongInputExitAudioUrl = tbWrongInputExitAudioUrl.Text,
                    WrongInputExitText = tbWrongInputExitText.Text
                };

                if (IsNew)
                {
                    _ivrSettingsRepository.Insert(ivrSettings);
                }
                else
                {
                    _ivrSettingsRepository.Update(Convert.ToInt32(CurrentLanguageId), ivrSettings);
                }

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }

        private bool ValidateParameters(int selectedLanguageId)
        {
            if (string.IsNullOrEmpty(tbWrongInputAudioUrl.Text) || string.IsNullOrEmpty(tbWrongInputText.Text) ||
                   string.IsNullOrEmpty(tbWrongInputExitAudioUrl.Text) || string.IsNullOrEmpty(tbWrongInputExitText.Text))
            {
                AddUserMessage(Strings.PleaseFillAllFields);
                return false;
            }

            if (IsNew && _ivrSettingsRepository.TryGetByLanguageId(selectedLanguageId) != null)
            {
                AddUserMessage(Strings.DublicateLanguageMessage);
                return false;
            }

            if (!IsNew && CurrentLanguageId != selectedLanguageId.ToString() && _ivrSettingsRepository.TryGetByLanguageId(selectedLanguageId) != null)
            {
                AddUserMessage(Strings.DublicateLanguageMessage);
                return false;
            }

            if (!Uri.TryCreate(tbWrongInputAudioUrl.Text, UriKind.Absolute, out var temp))
            {
                AddUserMessage(Strings.WrongInputAudioUrlIsIncorrect);
                return false;
            }

            if (!Uri.TryCreate(tbWrongInputExitAudioUrl.Text, UriKind.Absolute, out temp))
            {
                AddUserMessage(Strings.WrongInputExitAudioUrlIsIncorrect);
                return false;
            }

            return true;
        }
    }
}