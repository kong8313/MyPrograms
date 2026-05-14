using System;
using System.Linq;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.ServerControls.Commands;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DialerFeaturesView : BaseForm
    {
        [StoreInViewState]
        protected int DialerId;

        private ISupervisorServiceClient _supervisorServiceClient;

        protected void Page_Load(object sender, EventArgs e)
        {
            dialogControl.OKButton.Visible = false;
            dialogControl.CancelButton.InnerText = "Close";
            if (!SupervisorPrincipal.Current.IsCatiDialerAdministrator)
                throw new Exception(Strings.ActionIsNotAllowed);

            if (!IsPostBack)
            {
                DialerId = int.Parse(Request["Id"]);
            }

            if (grid.GetCommand("OverrideDefaultValue") is OverlayCommand cmd)
                cmd.InlineParams = $"DialerId={DialerId}";

            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();
            grid.GetPage = GetPage;
        }

        /// <summary>
        /// Returns page of information to show in grid.
        /// </summary>
        protected object GetPage(out int totalCount)
        {
            var i = 0;
            //new object just for make new field for save original sorting
            var list = _supervisorServiceClient.GetOverridenDialerSupportedFeatures(DialerId).Select(x=> new
            {
                Id = i++,
                Name = x.Name,
                DefaultValue = x.DefaultValue.HasValue ? x.DefaultValue.Value.ToString() : Strings.NoInfo,
                OverridenValue = x.OverridenValue
            });
            return BaseMethods.GetPage(list, grid.PageArguments, out totalCount);
        }

        protected void DeleteOverriddenValue(object sender, EventArgs e)
        {
            if (grid.SelectedKeys.Length == 0)
            {
                return;
            }

            var name = grid.SelectedKeys[0];
            _supervisorServiceClient.UpdateOverridenDialerSupportedFeature(DialerId, name, null);
        }

    }
}