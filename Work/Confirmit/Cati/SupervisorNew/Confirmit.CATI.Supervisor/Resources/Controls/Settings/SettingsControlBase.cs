using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.ServerControls;
using Confirmit.CATI.Supervisor.ServerControls.Confirmit;
using System;
using Confirmit.CATI.Supervisor.Controls;

namespace Confirmit.CATI.Supervisor.Resources.Controls.Settings
{
    public abstract class SettingsControlBase : BaseWUC
    {
        public abstract GeneralToolbar Toolbar { get; }

        public abstract XpMenuItem SaveButton { get; }

        public abstract System.Web.UI.WebControls.Button DefaultButton { get; }

        public StateChecker StateChecker { get; set; }

        public ISystemSettings SystemSettings { get; set; }

        public EventHandler SaveClickHandler { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillSettings();
            }

            StateChecker.AddSaveButton(SaveButton);

            SaveButton.Click += SaveClickHandler;
            DefaultButton.Click += SaveClickHandler;
        }

        public abstract void FillSettings();

        public abstract void SaveSettings();

        public virtual void Validate()
        {
        }

    }
}
