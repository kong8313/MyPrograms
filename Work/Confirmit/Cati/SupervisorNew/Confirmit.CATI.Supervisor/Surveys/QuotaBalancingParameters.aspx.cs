using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Repositories;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces.Survey.Quota.Data;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Confirmit;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;

namespace Confirmit.CATI.Supervisor.Surveys
{
    [CheckSurveyPermission(RequestParameterName = "SurveyId")]
    public partial class QuotaBalancingParameters : BaseForm
    {
        private readonly IQuotaBalancingService _quotaBalancingService = ServiceLocator.Resolve<IQuotaBalancingService>();

        [StoreInViewState]
        protected int SurveyId;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SurveyId = int.Parse(Request["SurveyId"]);
                var configuration = _quotaBalancingService.GetQuotaBalancingConfiguration(SurveyId);
                config.Value = JsonConvert.SerializeObject(configuration);

                var balancedQuota = configuration.Quotas.FirstOrDefault();
                if (balancedQuota != null)
                {
                    nePriority.Value = configuration.PromotionPriority;
                    neThreshold.Value = configuration.PromotionThreshold;
                }
            }

            neThreshold.MaxDecimalPlaces = 0;
            nePriority.MaxDecimalPlaces = 0;
        }

        
        protected void OkButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var configuration = JsonConvert.DeserializeObject<QuotaBalancingConfiguration>(config.Value);
                configuration.PromotionPriority = nePriority.ValueInt;
                configuration.PromotionThreshold = neThreshold.ValueInt;
                _quotaBalancingService.SetQuotaBalancingConfiguration(SurveyId, configuration);

                string quotaNames = string.Join(", ", QuotaManager.GetBalancedQuotaNames(SurveyId));

                CloseOverlay(true, quotaNames);
            }
            catch (Exception ex)
            {
                Context.AddError(ex);
            }
        }
    }

}