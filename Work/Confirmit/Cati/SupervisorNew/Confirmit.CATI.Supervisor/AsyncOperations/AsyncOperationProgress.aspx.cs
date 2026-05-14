using System;
using System.IO;
using System.Resources;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Core.Timezones;
using Newtonsoft.Json;
using Confirmit.CATI.Supervisor.Core.Timezone;
using Confirmit.CATI.Supervisor.Resources;

namespace Confirmit.CATI.Supervisor.AsyncOperations
{
    public partial class AsyncOperationProgress : BaseForm
    {
        [StoreInViewState] 
        public int OperationId;

        [StoreInViewState] 
        public int RefreshRate;

        [StoreInViewState]
        public bool IsOpenedFromList;

        [StoreInViewState]
        public string DialogTitle;
        public override string Title {
            get
            {
                return DialogTitle;
            }
        } 

        protected void Page_Load(object sender, EventArgs e)
        {
            dialog.CancelButton.InnerText = "Close";
            dialog.CancelButton.Attributes["onclick"] = "CloseButtonHandler()";

            if (IsPostBack == false)
            {                
                Initialize();
                
                lblOperationId.Text = OperationId.ToString();

                if (IsOpenedFromList == false)
                {
                    ResizeWindow(520, 340);
                }
                else
                {
                    cbCloseOnFinish.Checked = false;
                    cbCloseOnFinish.Enabled = false;
                    divCloseOnFinish.Visible = false;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {                        
            RegisterStyleSheet("styles/AsyncOperationProgress.css");
            RegisterClientLibrary("AsyncOperations/client/AsyncOperationProgress.js");
            ClientScript.RegisterStartupScript(
                GetType(),
                "InitAsyncOperationProgress",
                string.Format(
                    "AsyncOperationProgress.init({0},{1},'{2}','{3}','{4}', '{5}','{6}');", 
                    OperationId, RefreshRate, lblText.ClientID, lblStartTime.ClientID, lblEndTime.ClientID, lblStatus.ClientID, cbCloseOnFinish.ClientID),
                true);

            RegisterStartupScript("Y.on('onbeforeunload', function(){BeforeDialogCloseHandler()});");

            if (IsOpenedFromList)
            {
                var value = new JavaScriptSerializer().Serialize(GetOperationProgress(OperationId));
                value = HttpUtility.JavaScriptStringEncode(value);

                RegisterStartupScript(String.Format("AsyncOperationProgress.setProgress(Sys.Serialization.JavaScriptSerializer.deserialize('{0}'));", value));
            }
        }
        
        [ScriptMethod, WebMethod(EnableSession = true)]
        public static AsyncOperationProgressInfo GetOperationProgress(int operationId)
        {
            var timezoneProvider = ServiceLocator.Resolve<ICachedLocalTimezoneManager>();
            var result = new AsyncOperationProgressInfo { OperationId = operationId };

            try
            {
                var repository = ServiceLocator.Resolve<IAsyncOperationRepository>();

                var operationInfo = repository.Get(operationId);

                result.Status = operationInfo.State;
                result.StatusDescription = ((AsyncOperationState) operationInfo.State).ToString();

                result.Text = operationInfo.Text != null ? 
                                operationInfo.Text.Replace(Environment.NewLine, "<br style='line-height: 150%'/>") : String.Empty;
                
                if (operationInfo.TotalItemsCount > 0)
                {
                    double percentComplete = (double)operationInfo.ProcessedItemsCount / (double)operationInfo.TotalItemsCount * 100;
                    result.PercentageComplete = (int)percentComplete;
                }

                if (operationInfo.StartedDate.HasValue)
                {
                    //if 'Local' kind is specified during serializing the time is written with an offset, if Utc it is written without.
                    var localStartTime = timezoneProvider.ConvertToLocalTime(operationInfo.StartedDate.Value);
                    result.StartTime = DateTime.SpecifyKind(localStartTime, DateTimeKind.Utc);
                }

                if (operationInfo.FinishedDate.HasValue)
                {
                    var localEndTime = timezoneProvider.ConvertToLocalTime(operationInfo.FinishedDate.Value);
                    result.EndTime = DateTime.SpecifyKind(localEndTime, DateTimeKind.Utc);
                }
            }
            catch (Exception ex)
            {
                result.IsStateRetrievalException = true;
                ExceptionTraceHelper.TraceException(ex);                
            }

           return result;         
        }

        private void Initialize()
        {            
            var escaper = new EscapeHelper();

            OperationId = Int32.Parse(Request["OperationId"]);
            RefreshRate = Request.QueryString["RefreshRate"] == null
                              ? 1
                              : Int32.Parse(Request.QueryString["RefreshRate"]);            

            IsOpenedFromList = Request.QueryString["IsOpenedFromList"] != null &&
                               bool.Parse(Request.QueryString["IsOpenedFromList"]);

            var title = string.Empty;
            var titleResource = escaper.EscapeString(Request["OperationTitle"]); 

            if (string.IsNullOrEmpty(titleResource))
            {
                var repository = ServiceLocator.Resolve<IAsyncOperationRepository>();
                var operationInfo = repository.Get(OperationId);
                title = operationInfo.Title;
            }
            else
            {
                var resourceManager = new ResourceManager(typeof(Strings));

                try
                {
                    title = resourceManager.GetString(titleResource);
                }
                catch (Exception e)
                {
                    ExceptionTraceHelper.TraceException(e);   
                }
            }
            
            lblOperationTitle.Text = title;
            if (string.IsNullOrWhiteSpace(title))
            {
                operationTitle.Visible = false;
            }
            var dialogTitle = HttpUtility.UrlDecode(Request["DialogTitle"] ?? string.Empty);
            if (!String.IsNullOrEmpty(dialogTitle))
            {
                DialogTitle = dialogTitle;
            }
        }
    }
}
