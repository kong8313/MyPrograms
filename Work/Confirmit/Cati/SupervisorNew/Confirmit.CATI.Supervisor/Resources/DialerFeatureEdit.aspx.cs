using System;
using System.Linq;
using System.Web.UI.WebControls;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Misc.CP;
using Confirmit.CATI.Core.SupervisorService;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Resources
{
    public partial class DialerFeatureEdit : BaseForm
    {
        [StoreInViewState]
        protected int DialerId;
        [StoreInViewState]
        protected string FeatureName;
        [StoreInViewState]
        protected string FeatureDefaultValue;
        [StoreInViewState]
        protected bool? FeatureOverridenValue;

        private ISupervisorServiceClient _supervisorServiceClient;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SupervisorPrincipal.Current.IsCatiDialerAdministrator)
                throw new Exception(Strings.ActionIsNotAllowed);

            _supervisorServiceClient = ServiceLocator.Resolve<ISupervisorServiceClient>();

            if (IsPostBack) return;

            if (Request["Name"] != null)
                FeatureName = Request["Name"];
            if (Request["DialerId"] != null)
                DialerId = Convert.ToInt32(Request["DialerId"]);

            var feature = _supervisorServiceClient.GetOverridenDialerSupportedFeatures(DialerId).FirstOrDefault(x => x.Name == FeatureName);

            if (feature == null) return;
            FeatureDefaultValue = feature.DefaultValue.HasValue ? feature.DefaultValue.Value.ToString() : Strings.NoInfo;
            FeatureOverridenValue = feature.OverridenValue;

            DataBind();

            FillValueList();

            featureHint.Text = Strings.OverridingFeatureValueInfo;

            dialog.CancelButton.Attributes["onclick"] = "if(parent.overlay.isOpen) parent.overlay.closeLast(); if(window.top.overlay.isOpen) top.overlay.closeLast();";
        }

        private void FillValueList()
        {
            ddlOverridenValue.Items.Add(new ListItem(Strings.OverridenFeatureValue_None, "none"));
            ddlOverridenValue.Items.Add(new ListItem(Strings.OverridenFeatureValue_False, "false"));
            ddlOverridenValue.Items.Add(new ListItem(Strings.OverridenFeatureValue_True, "true"));
            ddlOverridenValue.SelectedValue = FeatureOverridenValue.HasValue ? FeatureOverridenValue.ToString().ToLower() : "none";
        }

        private void HideInputAndSubmitButton()
        {
            trOverridenValue.Visible = false;
            dialog.OKButton.Visible = false;
        }

        protected void Save(object sender, EventArgs e)
        {
            var value = default(bool?);
            if (bool.TryParse(ddlOverridenValue.SelectedValue, out var parsedResult))
                value = parsedResult;

            _supervisorServiceClient.UpdateOverridenDialerSupportedFeature(DialerId, FeatureName, value);

            CloseOverlay(true, null, true);
        }
    }
}