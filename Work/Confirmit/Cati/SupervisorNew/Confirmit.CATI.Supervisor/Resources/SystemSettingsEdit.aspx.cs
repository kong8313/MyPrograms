using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls;
using Infragistics.Web.UI.EditorControls;
using Microsoft.Rest;
using TextBox = Confirmit.CATI.Supervisor.ServerControls.TextBox;
using DropDownList = Confirmit.CATI.Supervisor.ServerControls.DropDownList;
using TimeSpan = System.TimeSpan;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class SystemSettingsEdit : BaseForm
    {
        private const string SettingValueControlId = "currentSettingValue";

        private static readonly string[] MultilineSettingNames = {
            SystemSettingConstants.Dialer.DefaultSurveyParameters,
            SystemSettingConstants.Dialer.SettingsTemplatesJson,
            SystemSettingConstants.Dialer.InboundAudioMessagesJson,
            SystemSettingConstants.Email.AdministratorEmailAddress,
            SystemSettingConstants.Email.NotificationEmailBCC,
            SystemSettingConstants.Email.NotificationEmailRecipients,
            SystemSettingConstants.Reports.CallHistoryReportRecepients,
            SystemSettingConstants.Reports.InterviewerProductivityReportRecepients,
            SystemSettingConstants.Reports.SurveyOverviewReportRecepients,
            SystemSettingConstants.Reports.SurveyProductivityReportRecepients
        };

        [StoreInViewState]
        protected string SystemName;
        [StoreInViewState]
        protected bool IsDefaultSetting;

        private readonly ISystemSettingRepository _systemSettingRepository = ServiceLocator.Resolve<ISystemSettingRepository>();
        private readonly ISqlTableUpdatedPublisher _publisher = ServiceLocator.Resolve<ISqlTableUpdatedPublisher>();
        private BvSystemSettingsEntity _defaultCompanySetting;
        private BvSystemSettingsEntity _overriddenCompanySetting;

        public BvSystemSettingsEntity DefaultCompanySetting => _defaultCompanySetting
                                                                ?? (_defaultCompanySetting = _systemSettingRepository.GetSettingForDefaultCompany(SystemName));

        public BvSystemSettingsEntity OverriddenCompanySetting => _overriddenCompanySetting
                                                               ?? (_overriddenCompanySetting = _systemSettingRepository.GetSettingForCurrentCompany(SystemName));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["SystemName"] != null)
            {
                SystemName = Request["SystemName"];
            }

            if (Request["IsDefaultSetting"] != null)
            {
                IsDefaultSetting = bool.Parse(Request["IsDefaultSetting"]);
            }

            DataBind();

            FillInputs();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var notOverridableSettingsGroups = Config.NotOverridableSystemSettingsGroups.Split(',');

            if (!IsDefaultSetting && notOverridableSettingsGroups.Contains(DefaultCompanySetting.Group) && 
                SystemName != "Server.BackendMinThreadPoolSize")
            {
                HideInputAndSubmitButton();
                systemSettingHint.Text = string.Format(Strings.NotOverridableSystemSettingsHintText,
                    DefaultCompanySetting.Group);
                return;
            }

            trHint.Visible = false;
        }

        private void HideInputAndSubmitButton()
        {
            trSettingValue.Visible = false;
            dialog.OKButton.Visible = false;
        }

        private void FillInputs()
        {
            var value = GetInputInitialValue();

            tdSettingValue
                .Controls
                .Add(GetControl(DefaultCompanySetting, value, SettingValueControlId));
        }

        private string GetInputInitialValue()
        {
            return IsDefaultSetting ? DefaultCompanySetting.Value : OverriddenCompanySetting != null ? OverriddenCompanySetting.Value : DefaultCompanySetting.Value;
        }

        protected void Save(object sender, EventArgs e)
        {
            var message = string.Empty;
            var updateSystemSettingsEvent = new UpdateSystemSettingsEvent();
            var newSettingValue = GetValueFromControl(tdSettingValue.FindControl(SettingValueControlId));

            if (IsDefaultSetting)
            {
                if (DefaultCompanySetting.Value != newSettingValue)
                {
                    message = string.Format(Strings.ValueOfDefaultSystemSettingWasChanged,
                        DefaultCompanySetting.SystemName,
                        DefaultCompanySetting.Value,
                        newSettingValue);
                    DefaultCompanySetting.Value = newSettingValue;
                    _systemSettingRepository.UpdateSettingForDefaultCompany(DefaultCompanySetting);
                    
                    _publisher.PublishSystemSettingsUpdatedInAllCompanies();
                }
            }
            else
            {
                if (OverriddenCompanySetting == null)
                {
                    var newCompanyEntity = new BvSystemSettingsEntity
                    {
                        SystemName = DefaultCompanySetting.SystemName,
                        Description = DefaultCompanySetting.Description,
                        DisplayName = DefaultCompanySetting.DisplayName,
                        Group = DefaultCompanySetting.Group,
                        Hidden = DefaultCompanySetting.Hidden,
                        Type = DefaultCompanySetting.Type,
                        Value = newSettingValue
                    };

                    message = string.Format(Strings.ValueOfOverriddenSystemSettingWasSetted,
                        newCompanyEntity.SystemName, newCompanyEntity.Value);
                    _systemSettingRepository.InsertSettingForCurrentCompany(newCompanyEntity);
                    
                    _publisher.PublishSystemSettingsUpdated();
                }
                else
                {
                    if (OverriddenCompanySetting.Value != newSettingValue)
                    {
                        message = string.Format(Strings.ValueOfOverriddenSystemSettingWasChanged,
                            DefaultCompanySetting.SystemName,
                            OverriddenCompanySetting.Value,
                            newSettingValue);
                        OverriddenCompanySetting.Value = newSettingValue;
                        _systemSettingRepository.UpdateSettingForCurrentCompany(OverriddenCompanySetting);
                        
                        _publisher.PublishSystemSettingsUpdated();
                    }
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                updateSystemSettingsEvent.Details.Messages.Add(message);
                updateSystemSettingsEvent.Finish();
            }

            CloseOverlay(true);
        }

        private string GetValueFromControl(Control control)
        {
            switch (control)
            {
                case DropDownList dropDownList:
                    return dropDownList.SelectedValue;
                case NumericEdit numericEdit:
                    var strNumValue = numericEdit.ValueText.Trim();
                    if (!int.TryParse(strNumValue, out _))
                    {
                        throw new ValidationException("The value must be an integer!");
                    }

                    return strNumValue;
                case WebMaskEditor webMaskEditor:
                    var strValue = webMaskEditor.Text.Trim();
                    if (!TimeSpan.TryParse(strValue, out var timeSpan))
                    {
                        throw new ValidationException("Invalid timespan!");
                    }

                    return $"{timeSpan:d\\.hh\\:mm\\:ss}";
                case TextBox textBox:
                    return textBox.Text.Trim();
                default:
                    throw new ValidationException("Unknown error.");
            }
        }

        public Control GetControl(BvSystemSettingsEntity setting, string value, string controlId)
        {
            switch ((SystemSettingValueType)setting.Type)
            {
                case SystemSettingValueType.Bool:
                    var firstListItem = new ListItem("True");
                    firstListItem.Selected = firstListItem.Text == value;
                    var secondListItem = new ListItem("False");
                    secondListItem.Selected = secondListItem.Text == value;

                    return new DropDownList
                    {
                        ID = controlId,
                        Items = { firstListItem, secondListItem }
                    };
                case SystemSettingValueType.Int:
                    return new NumericEdit
                    {
                        ID = controlId,
                        ValueText = !string.IsNullOrEmpty(value) ? value : "0",
                        MinValue = int.MinValue,
                        MaxValue = int.MaxValue,
                        Nullable = false,
                        HorizontalAlign = HorizontalAlign.Left,
                        Buttons =
                        {
                            SpinButtonsDisplay = ButtonDisplay.None
                        }
                    };
                case SystemSettingValueType.Timespan:
                    var timeSpan = !string.IsNullOrEmpty(value) ? TimeSpan.Parse(value) : TimeSpan.Zero;
                    var holder = new Panel();
                    holder.Controls.Add(
                        new WebMaskEditor
                        {
                            ID = controlId,
                            HorizontalAlign = HorizontalAlign.Left,
                            Width = 150,
                            InputMask = "9999.99:99:99",
                            PadChar = '0',
                            RawText = $"{timeSpan:ddddhhmmss}"
                        });
                    holder.Controls.Add(new Label { Text = @"dddd.hh:mm:ss" });
                    return holder;
                case SystemSettingValueType.String:
                default:
                    return IsMultilineString(setting.SystemName) ?
                        new MultilineTextBox
                        {
                            ID = controlId,
                            Text = value,
                            Rows = 4
                        } :
                        new TextBox
                        {
                            ID = controlId,
                            Text = value
                        };
            }
        }

        private bool IsMultilineString(string settingName)
        {
            return MultilineSettingNames.Contains(settingName);
        }
    }
}