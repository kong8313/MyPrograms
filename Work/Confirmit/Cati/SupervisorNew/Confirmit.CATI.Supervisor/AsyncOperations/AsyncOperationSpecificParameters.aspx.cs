using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.AsyncOperations.Operations;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Supervisor.Classes;
using Confirmit.CATI.Core.Timezones;
using Confirmit.CATI.Supervisor.Core.Activity;
using System.Xml.Serialization;
using Confirmit.CATI.Supervisor.Core.Common;
using Confirmit.CATI.Supervisor.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Confirmit.CATI.Supervisor.AsyncOperations
{
    public partial class AsyncOperationSpecificParameters : BaseForm
    {
        private readonly List<KeyValuePair<String, String>> _operationDetails = new List<KeyValuePair<string, string>>();
        private readonly IAsyncOperationRepository _asyncOperationRepository = ServiceLocator.Resolve<IAsyncOperationRepository>();
        private readonly IAsyncOperationFactory _asyncOperationFactory = ServiceLocator.Resolve<IAsyncOperationFactory>();
            
        [StoreInViewState] 
        public int OperationId;
        
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
            if (IsPostBack == false)
            {
                OperationId = Int32.Parse(Request["OperationId"]);
            }

            var operation = _asyncOperationRepository.Get(OperationId);

            BindRepeator(operation);
            BindError(operation);
        }
        
        private void BindRepeator(BvAsyncOperationQueueEntity operation)
        {            
            var descriptor = _asyncOperationFactory.GetOperationDescriptorFromOperationType((OperationTypes)operation.Type);

            var obj = DeserializeParameters(operation.Parameters, descriptor.OperationParametersType);

            repeater.DataSource = GetFields(obj);
            repeater.DataBind();
        }

        private void BindError(BvAsyncOperationQueueEntity operation)
        {
            if (String.IsNullOrEmpty(operation.Error) == false)
            {
                trError.Visible = true;
                tbErrorValue.Text = operation.Error;    
            }            
        }        

        private IEnumerable<KeyValuePair<string, string>> GetFields(object obj)
        {            
            var result = new List<KeyValuePair<string, string>>();

            var fields =  obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray();

            foreach (var field in fields)
            {
                var value = JsonConvert.SerializeObject(field.GetValue(obj, null));
                result.Add(new KeyValuePair<string, string>(field.Name, value));                
            }

            return result;
        }

        private object DeserializeParameters(string parameters, Type parametersType)
        {
            var serializer = new XmlSerializer(parametersType);

            using (var reader = new StringReader(parameters))
            {
                return serializer.Deserialize(reader);
            }
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
