using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ActivityLogging;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.RabbitMQ.SqlTableUpdated;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class SystemSettings : BaseForm
    {
        public override string TopTitle => Strings.SystemSettings;

        private readonly ISystemSettingRepository _systemSettingRepository =
            ServiceLocator.Resolve<ISystemSettingRepository>();

        private readonly ISqlTableUpdatedPublisher _publisher = ServiceLocator.Resolve<ISqlTableUpdatedPublisher>();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            FillGroupsList();

            settingsGrid.GetPage += GetPage;
            settingsGrid.EnablePaging = false;

            if (!SupervisorPrincipal.Current.IsSystemAdministrator)
            {
                settingsGrid.HideCommand("ChangeDefaultValue");
            }
        }

        private object GetPage(out int totalCount)
        {
            var defaultCompanySettings = _systemSettingRepository.GetAllSettingsForDefaultCompany()
                .ToDictionary(x => x.SystemName);
            var currentCompanySettings = _systemSettingRepository.GetAllSettingsForCurrentCompany()
                .ToDictionary(x => x.SystemName);

            var systemSettingsList = defaultCompanySettings.Select(entity => new
            {
                entity.Value.SystemName,
                entity.Value.Group,
                entity.Value.DisplayName,
                entity.Value.Description,
                OverriddenValue = currentCompanySettings.ContainsKey(entity.Key)
                    ? currentCompanySettings[entity.Key].Value
                    : string.Empty,
                DefaultValue = entity.Value.Value
            }).ToList();

            totalCount = systemSettingsList.Count;

            systemSettingsList =
                BaseMethods.FilterCollection(systemSettingsList, settingsGrid.SearchParameterCollection);
            systemSettingsList.Sort(new CommonComparer<object>(settingsGrid.SortedColumnKey,
                settingsGrid.SortIndicatorAsc));

            return systemSettingsList;
        }

        private void FillGroupsList()
        {
            if (settingsGrid.Columns.FromKey("Group") is GeneralGridColumn groupColumn)
            {
                var groups = _systemSettingRepository.GetAllSettingsForDefaultCompany()
                    .Select(item => item.Group)
                    .Distinct()
                    .OrderBy(item => item);

                groupColumn.Items.AddRange(groups.Select(group => new ListItem(group)));
            }
        }

        protected void DeleteOverriddenValue(object sender, EventArgs e)
        {
            if (settingsGrid.SelectedKeys.Length == 0)
            {
                return;
            }

            var systemName = settingsGrid.SelectedKeys[0];
            var group = _systemSettingRepository.GetSettingForDefaultCompany(systemName).Group;
            var notOverridableSettingsGroups = Config.NotOverridableSystemSettingsGroups.Split(',');

            if (notOverridableSettingsGroups.Contains(group) && systemName != "Server.BackendMinThreadPoolSize")
            {
                ShowClientMessage(string.Format(Strings.NotOverridableSystemSettingsHintText,
                    group));
                return;
            }

            var updateSystemSettingsEvent = new UpdateSystemSettingsEvent();

            _systemSettingRepository.DeleteSettingForCurrentCompany(systemName);
            updateSystemSettingsEvent
                .Details
                .Messages
                .Add(string.Format(Strings.ValueOfOverriddenSystemSettingWasRevertedToDefault, systemName));
            updateSystemSettingsEvent.Finish();
            
            _publisher.PublishSystemSettingsUpdated();
        }
    }
}