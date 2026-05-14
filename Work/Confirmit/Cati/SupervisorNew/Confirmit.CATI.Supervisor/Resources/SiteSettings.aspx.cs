using Confirmit.CATI.Core.ActivityLogging.SiteSettings;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Resources.Controls.Settings;
using System;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Services;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class SiteSettings : BaseForm
    {
        private readonly ISystemSettings _systemSettings;
        private readonly List<SettingsControlBase> _settingsControls;
        private readonly ISqlTableUpdatedPublisher _sqlTableUpdatedPublisher;
        
        public override string TopTitle
        {
            get
            {
                return Strings.SiteSettings;
            }
        }

        public SiteSettings()
        {
            _systemSettings = ServiceLocator.Resolve<ISystemSettings>();
            _settingsControls = new List<SettingsControlBase>();
            _sqlTableUpdatedPublisher = ServiceLocator.Resolve<ISqlTableUpdatedPublisher>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!_systemSettings.Toggle.EnableMonitoringCoachingMode && !_systemSettings.Toggle.EnableMonitoringBargingMode)
            {
                tabs.GetTabByKey("MonitoringConsole").Hidden = true;
            }

            string tabKey = MaintainTabHelper.GetTabKey(ViewWithTabs.SiteSettings);
            if (!String.IsNullOrWhiteSpace(tabKey))
            {
                tabs.SelectTabByKey(tabKey);
            }

            tabs.ClientEvents.SelectedIndexChanged = "SelectedIndexChanged";

            InitSettingsControls();

            Page.ClientScript.RegisterOnSubmitStatement(GetType(), "EnableAllControls", "EnableAllControls();");
        }

        private void InitSettingsControls()
        {
            _settingsControls.Add(generalSettings);
            _settingsControls.Add(interviewerConsoleSettings);
            _settingsControls.Add(monitoringConsoleSettings);
            _settingsControls.Add(securitySettings);

            foreach (var settingsControl in _settingsControls)
            {
                settingsControl.SystemSettings = _systemSettings;
                settingsControl.StateChecker = stateChecker;
                settingsControl.SaveClickHandler = SaveButtonOnClick;
            }
        }

        protected override void ValidateForm()
        {
            /* NEED TO BE EMPTY */
        }

        protected void SaveButtonOnClick(object sender, EventArgs eventArgs)
        {
            SaveSiteSettings();
        }

        private void SaveSiteSettings()
        {
            try
            {
                if (IsValid)
                {
                    PerformSettingsSaving();
                    stateChecker.MarkAsUnchanged();
                }
            }
            catch (Exception error)
            {
                stateChecker.MarkAsChanged();
                Context.AddError(error);
            }
        }

        public void PerformSettingsSaving()
        {
            var events = new List<UpdateSiteSettingsEventBase>
            {
                new UpdateGeneralSiteSettingsEvent(),
                new UpdateInterviewerConsoleSiteSettingsEvent(),
                new UpdateSecuritySiteSettingsEvent()
            };
            events.ForEach(e => e.RememberSettings(_systemSettings));

            foreach (var settingsControl in _settingsControls)
            {
                settingsControl.SaveSettings();
            }
            
            _sqlTableUpdatedPublisher.PublishSystemSettingsUpdated();
            
            events.ForEach(e =>
            {
                e.CollectChangedSettings(_systemSettings);
                if (e.HasChanges)
                {
                    e.Finish();
                }
            });

        }

        public override void Validate()
        {
            foreach (var settingsControl in _settingsControls)
            {
                settingsControl.Validate();
            }

            base.Validate();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public static void SetSelectedTab(string tabKey)
        {
            MaintainTabHelper.SetTabKey(ViewWithTabs.SiteSettings, tabKey);
        }

    }
}
