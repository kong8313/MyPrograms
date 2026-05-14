using Confirmit.CATI.Common;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Web.UI.WebControls;

namespace Confirmit.CATI.Supervisor.ServerControls
{
    public sealed class DialTypeDropDownList : DropDownList
    {
        private const int NoChange = -1;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (AddAllOption)
            {
                Items.Add(new ListItem(Strings.All, Strings.All, true));
            }

            if (AddNoChangeOption)
            {
                Items.Add(new ListItem(Strings.NoChange, NoChange.ToString(), true));
            }

            var toggleSettings = ServiceLocator.Resolve<IToggleSettings>();

            foreach (var dialType in DialTypeOptions.GetAllowed())
            {
                Items.Add(new ListItem(dialType.ToString(), ((int)dialType).ToString()));
            }
        }

        public DialType? SelectedDialType
        {
            get
            {
                if (SelectedValue == NoChange.ToString())
                {
                    return null;
                }

                return (DialType)byte.Parse(SelectedValue);
            }
            set
            {
                SelectedValue = value.ToString();
            }
        }

        public bool AddNoChangeOption { get; set; }
        public bool AddAllOption { get; set; }
    }
}