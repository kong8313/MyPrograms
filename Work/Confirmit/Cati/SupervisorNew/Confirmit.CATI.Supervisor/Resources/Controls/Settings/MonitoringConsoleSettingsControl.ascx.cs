using Confirmit.CATI.Common;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;
using System.Web.UI.WebControls;
using Confirmit.CATI.Supervisor.Controls;

namespace Confirmit.CATI.Supervisor.Resources.Controls.Settings
{
    public partial class MonitoringConsoleSettingsControl : SettingsControlBase
    {
        public override GeneralToolbar Toolbar
        {
            get { return toolbar; }
        }

        public override XpMenuItem SaveButton
        {
            get { return btnSaveProperties; }
        }

        public override Button DefaultButton
        {
            get { return btnDefault; }
        }

        public override void FillSettings()
        {
            var monitoringSettings = SystemSettings.Monitoring;

            if (SystemSettings.Toggle.EnableMonitoringCoachingMode)
            {
                coachingRow.Visible = true;
                cdAllowMonitoringCoachingMode.Checked = monitoringSettings.AllowCoachingMode;
            }
            else
            {
                coachingRow.Visible = false;
            }

            if (SystemSettings.Toggle.EnableMonitoringBargingMode)
            {
                bargingRow.Visible = true;
                cdAllowMonitoringBargingMode.Checked = monitoringSettings.AllowBargingMode;
            }
            else
            {
                bargingRow.Visible = false;
            }
        }

        public override void SaveSettings()
        {
            var monitoringSettings = SystemSettings.Monitoring;

            using (var transactionScope = new DatabaseTransactionScope("SetConsoleSettings", DeadlockPriority.Supervisor))
            {
                if (SystemSettings.Toggle.EnableMonitoringCoachingMode)
                {
                    monitoringSettings.AllowCoachingMode = cdAllowMonitoringCoachingMode.Checked;
                }

                if (SystemSettings.Toggle.EnableMonitoringBargingMode)
                {
                    monitoringSettings.AllowBargingMode = cdAllowMonitoringBargingMode.Checked;
                }

                transactionScope.Commit();
            }
        }
    }
}