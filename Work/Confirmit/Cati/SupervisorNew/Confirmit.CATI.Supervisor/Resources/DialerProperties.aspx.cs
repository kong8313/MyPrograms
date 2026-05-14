using Confirmit.CATI.Common;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Classes.DialerConfiguration;
using Confirmit.CATI.Supervisor.Core.Common;
using ConfirmitDialerInterface;
using DialerCommon;
using DialerCommon.DialerParameters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DialerProperties : BaseForm
    {
        [StoreInViewState]
        protected DialerConfigurationType DialerType;

        [StoreInViewState]
        protected int Id;

        [StoreInViewState]
        protected DialerSettingTemplate DialerSettings;

        protected bool IsNew => Id == 0;

        private IToggleSettings _toggleSettings;
        private IDialersRepository _dialersRepository;
        private ICompanyInfo _companyInfo;
        private IDialerStatusProvider _dialerStatusProvider;
        private DialerConfigurationConverter _converter;
        private IDialerSettings _dialerSettings;

        private string ConvertToDialTypeDescription(byte dialTypeId)
        {
            if (Enum.IsDefined(typeof(DialType), (int)dialTypeId))
            {
                switch ((DialType)dialTypeId)
                {
                    case DialType.Landline:
                        return "Automatic";
                    case DialType.Cellphone:
                        return "Manual";
                    case DialType.Assisted:
                        return "Assisted";
                }
            }
            return "Undefined";
        }

        private DialType ConvertFromDialTypeDescription(string dialType)
        {
            switch (dialType)
            {
                case "Automatic":
                    return DialType.Landline;
                case "Manual":
                    return DialType.Cellphone;
                case "Assisted":
                    return DialType.Assisted;
                default:
                    throw new Exception($"Dial type {dialType} was not found");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SupervisorPrincipal.Current.IsCatiDialerAdministrator)
            {
                throw new Exception(Strings.ActionIsNotAllowed);
            }

            var encryptor = ServiceLocator.Resolve<IDialerAuthorizationKeyEncryptor>();
            _dialersRepository = ServiceLocator.Resolve<IDialersRepository>();
            _companyInfo = ServiceLocator.Resolve<ICompanyInfo>();
            _dialerStatusProvider = ServiceLocator.Resolve<IDialerStatusProvider>();
            _dialerSettings = ServiceLocator.Resolve<IDialerSettings>();
            _toggleSettings = ServiceLocator.Resolve<IToggleSettings>();
            _converter = new DialerConfigurationConverter(_dialerSettings, encryptor);

            if (!IsPostBack)
            {
                int.TryParse(Request["Id"], out Id);

                InitDialerTypeDropDown();
                InitDialTypeDropDown();

                propertiesHint.Visible = false;
                dialerIsActiveHint.Visible = false;
                reconnectDuration.Value = "02:00:00";
                reconnectAutomatically.Checked = true;

                if (!IsNew)
                {
                    var dialer = _dialersRepository.GetById(Id);

                    tbId.Text = dialer.Id.ToString();
                    tbId.Enabled = false;
                    tbName.Text = dialer.Name;
                    ddlDialType.SelectedIndex = dialer.DialTypeId;
                    tbWhitelist.Text = dialer.WhiteList;

                    reconnectAutomatically.Checked = dialer.ReconnectionDuration != null;

                    if (reconnectAutomatically.Checked)
                    {
                        reconnectDuration.Value = DateTime.Today.AddMilliseconds((double)dialer.ReconnectionDuration).ToString("HH:mm");
                    }

                    //it should not be possible to change dialer type after it has been saved
                    ddlDialerType.Enabled = dialer.DialerConfigurationTypeId == null;

                    DialerConfigurationType? configurationType;
                    DialerSettings = _converter.FromXmlToDialerSettingTemplate(dialer);
                    if (dialer.DialerConfigurationTypeId.HasValue)
                    {
                        configurationType = (DialerConfigurationType)dialer.DialerConfigurationTypeId;
                        var type = DialerConfigurationConverter.DialerTypesMap[configurationType.Value];
                        _converter.MergeWithTemplate(DialerSettings, configurationType.Value);
                        DialerSettings.DialerType = type;
                    }
                    else if (_converter.TryGetDialerType(DialerSettings, out var type, out configurationType))
                    {
                        _converter.MergeWithTemplate(DialerSettings, type.Value);
                        DialerSettings.DialerType = type.Value;
                    }
                    else
                    {
                        configurationType = null;
                        propertiesHint.Visible = true;
                    }

                    if (_dialerStatusProvider.GetDialerStatus(dialer.Id, dialer.IsActive) != DialerStatus.DisconnectedAndDeactivated)
                    {
                        dialerIsActiveHint.Visible = true;
                    }

                    if (configurationType.HasValue)
                    {
                        var selectedValue = DialerConfigurationConverter.DialerTypesMap.FirstOrDefault(x =>
                            x.Value == DialerSettings.DialerType && x.Key.ToString().StartsWith(configurationType.ToString(), StringComparison.OrdinalIgnoreCase));

                        ddlDialerType.SelectedIndex = (int)selectedValue.Key;
                    }
                    else
                    {
                        ddlDialerType.SelectedIndex = 0;
                    }
                }

                UpdateDialerType();
            }

            dialog.OKButton.Text = IsNew ? Strings.Add : Strings.Save;

            if (_toggleSettings.UseNewDialerApi && !IsNew)
            {
                var serviceAddress = DialerSettings.DialerConnectionParameters.FirstOrDefault(x => x.Id == "ServiceAddress")?.Value;
                dialog.OKButton.OnClientClick = $"alertIfServiceAddressChanged('{serviceAddress ?? ""}')";
            }
        }

        private void InitDialTypeDropDown()
        {
            var dropDownData = new List<ListItem>();

            foreach (var dialType in DialTypeOptions.GetAllowed())
            {
                dropDownData.Add(new ListItem(ConvertToDialTypeDescription((byte)dialType)));
            }

            ddlDialType.DataSource = dropDownData;
            ddlDialType.DataBind();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            dialog.OKButton.Enabled = ddlDialerType.SelectedIndex != -1 && !dialerIsActiveHint.Visible;
        }

        private void InitDialerTypeDropDown()
        {
            var dropDownData =
                Enum.GetValues(typeof(DialerConfigurationType)).Cast<DialerConfigurationType>()
                    .Select(x => new ListItem(DialerConfigurationTypeString(x), x.ToString())).ToList();

            if (!IsNew)
            {
                dropDownData.Insert(0, new ListItem(Strings.Choose, Strings.Choose));
            }

            ddlDialerType.DataSource = dropDownData;
            ddlDialerType.DataTextField = "Text";
            ddlDialerType.DataValueField = "Value";
            ddlDialerType.DataBind();
        }

        private string DialerConfigurationTypeString(DialerConfigurationType type)
        {
            var openDialerApiTypes = new List<DialerConfigurationType>
            {
                DialerConfigurationType.Sytel,
                DialerConfigurationType.InVade,
                DialerConfigurationType.Simulator,
                DialerConfigurationType.AmazonConnect
            };

            var typeAsString = type.ToString();

            if (openDialerApiTypes.Contains(type))
            {
                typeAsString += " (Open Dialer API)";
            }

            return typeAsString;
        }

        protected void OnClick(object sender, EventArgs e)
        {
            try
            {
                FillDialerParameters();

                var name = tbName.Text.Trim();
                var whiteList = tbWhitelist.Text.Trim();
                uint.TryParse(tbId.Text, out var id);
                var dialType = ConvertFromDialTypeDescription(ddlDialType.SelectedValue);
                var dialerType = DialerConfigurationConverter.DialerTypesMap[DialerType].ToString();

                if (_dialerSettings.DialerType != dialerType && _dialerSettings.DialerType != DiallerType.NoDialler.ToString())
                {
                    throw new UserMessageException(string.Format(Strings.InconsistentDialerTypeError, _dialerSettings.DialerType, dialerType));
                }

                if (string.IsNullOrEmpty(name))
                {
                    throw new UserMessageException(Strings.Err_EmptyName);
                }

                if (id == 0)
                {
                    throw new UserMessageException(Strings.Err_EmptyId);
                }

                var configParamsXml = _converter.GetDialerConfigurationParametersXml(DialerSettings);
                var connectionParamsXml = _converter.GetDialerConnectionParametersXml(DialerSettings, IsNew);



                if (IsNew)
                {
                    var anyDialerExist = _dialersRepository.GetAll().Any();
                    AddDialer(configParamsXml, connectionParamsXml, name, dialType, whiteList, GetReconnectionDuration());
                    if (!anyDialerExist)
                    {
                        _dialerSettings.DefaultSurveyParameters = _converter.GetDialerSurveyParametersXml(DialerSettings);
                    }
                }
                else
                {
                    UpdateDialer(configParamsXml, connectionParamsXml, name, dialType, whiteList, GetReconnectionDuration());
                }

                _dialerSettings.DialerType = dialerType;

                hrBeforeConfigParams.Visible = false;
                hrBeforeConnectionParams.Visible = false;

                CloseOverlay(true);
            }
            catch (Exception ex)
            {
                UpdateDialerType();
                Context.AddError(ex);
            }
        }

        private double? GetReconnectionDuration()
        {
            double? result = null;
            if (reconnectAutomatically.Checked && DateTime.TryParse(reconnectDuration.Value.ToString(), out var duration))
            {
                var minDuration = TimeSpan.FromMinutes(5).TotalMilliseconds;
                result = duration.TimeOfDay.TotalMilliseconds;
                if (result < minDuration)
                {
                    reconnectDuration.Value = minDuration;
                    result = minDuration;
                }
            }
            return result;
        }

        private void FillDialerParameters()
        {
            foreach (var configProperty in DialerSettings.DialerConfigurationParameters)
            {
                configProperty.Value = GetValueForProperty(configProperty);
            }

            foreach (var configProperty in DialerSettings.DialerConnectionParameters)
            {
                if (configProperty.Id == "AuthorizationKeyForOutgoingRequests")
                {
                    continue;
                }

                configProperty.Value = GetValueForProperty(configProperty);
            }

            var serviceEndpoint = DialerSettings.DialerConnectionParameters.FirstOrDefault(x => x.Id == "ServiceEndpoint");
            if (serviceEndpoint != null)
            {
                switch (DialerSettings.DialerType)
                {
                    case DiallerType.BvTCI:
                        serviceEndpoint.Value = "BvTciDialerServiceEndpoint";
                        break;
                    case DiallerType.PROTS:
                        serviceEndpoint.Value = "PROTSDialerServiceEndpoint";
                        break;
                    default:
                        var serviceAddress = DialerSettings.DialerConnectionParameters.FirstOrDefault(x => x.Id == "ServiceAddress");
                        if (serviceAddress != null)
                        {
                            var url = new Uri(serviceAddress.Value);

                            serviceEndpoint.Value = url.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)
                                ? "DialerServiceEndpointHttps"
                                : "DialerServiceEndpointHttp";
                        }
                        break;
                }
            }
        }

        private void UpdateDialer(string configParamsXml, string connectionParamsXml, string name, DialType dialType,
            string whiteList, double? ReconnectionDuration)
        {
            var dialerEntity = _dialersRepository.GetById(Id);

            var evt = new EditDialerEvent(dialerEntity.Clone());

            dialerEntity.ConfigurationParameters = configParamsXml;
            dialerEntity.ConnectionParameters = connectionParamsXml;
            dialerEntity.Name = name;
            dialerEntity.DialTypeId = (byte)dialType;
            dialerEntity.WhiteList = whiteList;
            dialerEntity.DialerConfigurationTypeId = (int?)DialerType;
            dialerEntity.ExpectedState = ReconnectionDuration == null
                ? (int)_dialerStatusProvider.GetDialerStatus(dialerEntity.Id, dialerEntity.IsActive)
                : dialerEntity.ExpectedState;
            dialerEntity.ReconnectionDuration = (int?)ReconnectionDuration;

            _dialersRepository.Update(dialerEntity);

            evt.Details.AfterChanging = dialerEntity;
            evt.Finish();
        }

        private void AddDialer(string configParamsXml, string connectionParamsXml, string name, DialType dialType,
            string whiteList, double? ReconnectionDuration)
        {
            var dialerEntity = new BvDialersEntity
            {
                Id = int.Parse(tbId.Text),
                ConfigurationParameters = configParamsXml,
                ConnectionParameters = connectionParamsXml,
                Name = name,
                TenantId = _companyInfo.CompanyId,
                DialTypeId = (byte)dialType,
                WhiteList = whiteList,
                DialerConfigurationTypeId = (int?)DialerType,
                ReconnectionDuration = (int?)ReconnectionDuration,
                ExpectedState = (int)DialerStatus.DisconnectedAndDeactivated
            };

            var evt = new AddDialerEvent(dialerEntity);

            _dialersRepository.AddDialer(dialerEntity);

            evt.Finish();
        }

        private string GetValueForProperty(DialerParameter configProperty)
        {
            var postfix = "";
            if (configProperty.Id == "SupportedPersonModes")
            {
                postfix = "[]";
            }

            var value = Request.Params.Get($"inputFor{configProperty.Id}{postfix}");
            value = value != null ? value.Trim() : "";

            switch (configProperty.Type)
            {
                case "System.Boolean":
                    value = value == "on" ? "True" : "False";
                    break;
                case "System.Int32":
                    if (int.TryParse(value, out var intValue))
                    {
                        return intValue.ToString();
                    }
                    else
                    {
                        throw new UserMessageException(string.Format(Strings.DialerParameterShouldBeIntegerError, configProperty.Id));
                    }
            }

            return value;
        }

        protected void ddlDialerType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsNew)
            {
                var dialer = _dialersRepository.GetById(Id);

                DialerSettings = _converter.FromXmlToDialerSettingTemplate(dialer);

                if (ddlDialerType.SelectedIndex != 0)
                {
                    DialerType = (DialerConfigurationType)Enum.Parse(typeof(DialerConfigurationType), ddlDialerType.SelectedItem.Value);

                    var type = DialerConfigurationConverter.DialerTypesMap[DialerType];
                    DialerSettings.DialerType = type;

                    _converter.MergeWithTemplate(DialerSettings, DialerType);
                }
            }

            UpdateDialerType();
        }

        private void UpdateDialerType()
        {
            if (!IsNew && ddlDialerType.SelectedIndex == 0)
            {
                DialerType = (DialerConfigurationType)(-1);
                hrBeforeConfigParams.Visible = hrBeforeConnectionParams.Visible = false;
                return;
            }

            var oldDialerType = DialerType;
            DialerType = (DialerConfigurationType)Enum.Parse(typeof(DialerConfigurationType), ddlDialerType.SelectedItem.Value);
            if (DialerSettings == null || (IsNew && oldDialerType != DialerType))
            {
                var settings = ServiceLocator.Resolve<ISystemSettings>().Dialer.SettingsTemplatesJson;
                var list = JsonConvert.DeserializeObject<DialerConfigurationList>(settings);
                var type = DialerConfigurationConverter.DialerTypesMap[DialerType];
                DialerSettings = list.DialerSettingTemplates.FirstOrDefault(x => x.DialerType == type && x.Name.StartsWith(DialerType.ToString(), StringComparison.OrdinalIgnoreCase));
            }

            hrBeforeConfigParams.Visible = DialerSettings.DialerConfigurationParameters.Any();
            hrBeforeConnectionParams.Visible = DialerSettings.DialerConnectionParameters.Any();

            FillDialerSettings(tbDialerConnectionParameters, DialerSettings.DialerConnectionParameters, true);
            FillDialerSettings(tbDialerConfigurationParameters, DialerSettings.DialerConfigurationParameters, false);
        }

        private void FillDialerSettings(HtmlTable htmlTable, List<DialerParameter> dialerParameters, bool isConnectionSection)
        {
            var label = isConnectionSection ? "Connection Parameters" : "Configuration Parameters";
            htmlTable.Rows.Clear();
            htmlTable.Rows.Add(new HtmlTableRow
            {
                Cells =
                {
                    new HtmlTableCell
                    {
                        InnerHtml = $"<strong>{label}</label>",
                        ColSpan = 2
                    }
                }
            });

            foreach (var configProperty in dialerParameters)
            {
                if (configProperty.Id == "ServiceEndpoint" || configProperty.Id == "AuthorizationKeyForOutgoingRequests")
                {
                    continue;
                }

                htmlTable.Rows.Add(GetTableRow(configProperty, isConnectionSection));
            }
        }

        private HtmlTableRow GetTableRow(DialerParameter parameter, bool isConnectionParameter = false)
        {
            var id = parameter.Id;
            var name = parameter.Name;
            var type = parameter.Type;
            var value = parameter.Value;

            var row = new HtmlTableRow { ID = $"rowFor{id}" };
            row.Cells.Add(GetLabelCell(type, name, id, isConnectionParameter));

            var editCell = new HtmlTableCell { ID = $"cellFor{id}" };

            if (type == typeof(Boolean).FullName)
            {
                value = value == "True" ? "checked" : "";
                editCell.InnerHtml = $"<div class='checkbox-selector-wrapper'><input type='checkbox' name='inputFor{id}' {value}/><label /></div>";
            }
            else if (type == typeof(Int32).FullName)
            {
                editCell.InnerHtml = $"<input type='number' name='inputFor{id}' value='{value}' class='plain_textbox settings-value-numeric'/>";
            }
            else if (id == "SupportedPersonModes")
            {
                var values = value.Split(',');
                var personModes =
                    Enum.GetValues(typeof(AgentTaskChoiceMode)).Cast<AgentTaskChoiceMode>()
                        .Where(x => x != AgentTaskChoiceMode.Choice)
                        .Select(x =>
                            $"<div class='task-choice-boxes'><div class='checkbox-selector-wrapper'><input type='checkbox' id='{x}' name='inputFor{id}[]' value='{x}' {(values.Contains(x.ToString()) ? "checked" : "")}/><label for='{x}'>{x}</label></div></div>")
                        .ToList();
                editCell.InnerHtml = $"<div class='task-choice-container'>{string.Join("", personModes)}</div>";
            }
            else
            {
                editCell.InnerHtml =
                    $"<input type='text' name='inputFor{id}' value='{value}' class='plain_textbox'/>";
            }

            row.Cells.Add(editCell);

            return row;
        }

        private static HtmlTableCell GetLabelCell(string type, string name, string id, bool isConnectionParameter)
        {
            var labelCell = new HtmlTableCell();
            labelCell.Attributes.Add("class",
                $"dialer-property-label {(string.IsNullOrEmpty(type) ? "dialer-property-label--warning" : "")} {(isConnectionParameter ? "dialer-property-label--connection" : "")}");
            labelCell.Controls.Add(new Label
            {
                Text = $"{name ?? id}",
                ToolTip = string.IsNullOrEmpty(type)
                    ? Strings.PropertyMissingInDialerTemplate
                    : string.Format(Strings.DialerParameterTypeLabel, type)
            });
            return labelCell;
        }
    }
}