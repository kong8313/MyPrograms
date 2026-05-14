using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services.Survey;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.AsyncOperations
{
    public partial class AsyncOperationParameters : BaseForm
    {
        private readonly List<KeyValuePair<String, String>> _operationDetails = new List<KeyValuePair<string, string>>();
        private readonly IAsyncOperationRepository _asyncOperationRepository = ServiceLocator.Resolve<IAsyncOperationRepository>();
        private readonly ICachedLocalTimezoneManager _timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
        private readonly ICompanyInfoProvider _companyInfoProvider = ServiceLocator.Resolve<ICompanyInfoProvider>();
        
        [StoreInViewState] 
        public int OperationId;
        
        [StoreInViewState]
        public string DialogTitle;

        public override string Title { get { return DialogTitle; } } 

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                OperationId = Int32.Parse(Request["OperationId"]);
            }
            
            var operation = _asyncOperationRepository.Get(OperationId);

            FillOperationDetails(operation);

            repeater.DataSource = _operationDetails;
            repeater.DataBind();            
        }

        private void FillOperationDetails(BvAsyncOperationQueueEntity operation)
        {            
            _operationDetails.Add(new KeyValuePair<string, string>(Strings.ID, operation.Id.ToString()));
            _operationDetails.Add(new KeyValuePair<string, string>(Strings.SurveyID, SurveyService.GetFormattedSurveyName( operation.SurveySid)));
            _operationDetails.Add(new KeyValuePair<string, string>("User ID", operation.CreatedBySupervisorName));       

            if (_companyInfoProvider.HasCallCentersAddon)
            {
                _operationDetails.Add(new KeyValuePair<string, string>("Call Center ID", operation.CallCenterId.ToString()));    
            }
            
            _operationDetails.Add(new KeyValuePair<string, string>("Task Type", StringHelper.GetStringFromEnum((OperationTypes)operation.Type)));
            _operationDetails.Add(new KeyValuePair<string, string>("Status", StringHelper.GetStringFromEnum((AsyncOperationState)operation.State)));
            _operationDetails.Add(new KeyValuePair<string, string>(Strings.InitiatedTime, ConvertToLocalTime(operation.QueuedDate)));
            _operationDetails.Add(new KeyValuePair<string, string>(Strings.StartTime, ConvertToLocalTime(operation.StartedDate)));
            _operationDetails.Add(new KeyValuePair<string, string>(Strings.FinishTime, ConvertToLocalTime(operation.FinishedDate)));            
            _operationDetails.Add(new KeyValuePair<string, string>("Aborted By", operation.AbortedBySupervisorName));

            if (String.IsNullOrEmpty(operation.Error) == false)
            {
                _operationDetails.Add(new KeyValuePair<string, string>("Error", "Error occured"));
            }
        }

        private string ConvertToLocalTime(DateTime? operationTime)
        {
            return operationTime.HasValue ? _timezoneProvider.ConvertToLocalTime(operationTime.Value).ToString() : string.Empty;
        }

        protected void repeater_ItemDataBound(object sender, RepeaterItemEventArgs args)
        {            
            if (args.Item.ItemType == ListItemType.Item || args.Item.ItemType == ListItemType.AlternatingItem)
            {
                var obj = (KeyValuePair<string, string>)args.Item.DataItem;

                var lblHeader = (Label)args.Item.FindControl("lblName");
                lblHeader.Text = obj.Key;

                var lblValue = (Label)args.Item.FindControl("lblValue");
                lblValue.Text = obj.Value;                
            }
        }
    }
}
