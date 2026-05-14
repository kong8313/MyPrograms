using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Confirmit;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "SurveyId")]
    public partial class QuotaClusteringParameters : BaseForm
    {
        [StoreInViewState]
        protected int SurveyId;

        [StoreInViewState]
        protected string QuotaName;

        private readonly IQuotaClusteringConfigurationService _quotaClusteringConfigurationService;


        public QuotaClusteringParameters()
        {
            _quotaClusteringConfigurationService = ServiceLocator.Resolve<IQuotaClusteringConfigurationService>();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveyId = int.Parse(Request["SurveyId"]);
                QuotaName = _quotaClusteringConfigurationService.GetConfiguration(SurveyId).QuotaName;
            }

            neThreshold.MaxDecimalPlaces = 0;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var configuration = _quotaClusteringConfigurationService.GetConfiguration(SurveyId);

                if (!String.IsNullOrEmpty(configuration.QuotaName))
                {
                    FillQuotaDropdown(configuration.QuotaName);
                    neThreshold.Value = configuration.LiveThreshod;
                }
                else
                {
                    FillQuotaDropdown(null);   
                }
            }
        }

        private void FillQuotaDropdown(string selectedQuotaName)
        {
            ddlQuotas.Items.Clear();

            var quotasDetails = QuotaManager.GetQuotaNamesAndIds(SurveyId);

            foreach (var quotaDetails in quotasDetails)
            {
                ddlQuotas.Items.Add(new ListItem(quotaDetails.Name, quotaDetails.Name)
                    {
                        Selected = !String.IsNullOrEmpty(selectedQuotaName) && (quotaDetails.Name == selectedQuotaName)
                    });
            }
        }

        protected void OkButtonClicked(object sender, EventArgs e)
        {
            try
            {
                string quotaName = ddlQuotas.SelectedItem.Text;

                var configuration = new QuotaClusteringConfiguration()
                                    {
                                        QuotaName = quotaName,
                                        LiveThreshod = neThreshold.ValueInt
                                    };

                _quotaClusteringConfigurationService.Configure(SurveyId, configuration );

                CloseOverlay(true, quotaName);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }

}