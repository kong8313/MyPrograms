using System;
using System.Data;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Confirmit.CATI.Core.DAL.Framework;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Misc.ConfirmitClientKey;
using Confirmit.CATI.Core.SurveyDataService;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.Configuration.Bootstrap;

namespace Confirmit.CATI.Core.WcfServices.Clients
{
    /// <summary>
    /// Client wrapper for Confirmit internal survey data service.
    /// </summary>
    public class SurveyDataService : ISurveyDataService
    {
        private readonly IConfirmitClientKeyProvider _confirmitClientKeyProvider;
        private readonly IWebServiceUrlSettings _webServiceUrlSettings;

        private volatile  FusionSurveyDataSoapClient _surveyDataService;

        public SurveyDataService(
            IConfirmitClientKeyProvider confirmitClientKeyProvider,
            IWebServiceUrlSettings webServiceUrlSettings)
        {
            _confirmitClientKeyProvider = confirmitClientKeyProvider;
            _webServiceUrlSettings = webServiceUrlSettings;

            InitializeClient();
        }

        private void InitializeClient()
        {
            HttpBindingBase binding;
            CustomBinding customBinding;
            
            var url = BootstrapConfig.IsContainerEnvironment 
                ? "http://internal-soap-14-api/confirmit/internalwebservices/14.0/FusionSurveyData.asmx" 
                : _webServiceUrlSettings.SurveyData;
            
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                binding = new BasicHttpsBinding
                {
                    MaxReceivedMessageSize = 16777216,
                    ReaderQuotas = { MaxArrayLength = 2147483647, MaxStringContentLength = 5242880, MaxDepth = 128 },
                };

                customBinding = new CustomBinding(binding);
                customBinding.Elements.Find<HttpsTransportBindingElement>().KeepAliveEnabled = false;
            }
            else
            {
                binding = new BasicHttpBinding
                {
                    MaxReceivedMessageSize = 16777216,
                    ReaderQuotas = { MaxArrayLength = 2147483647, MaxStringContentLength = 5242880, MaxDepth = 128 }
                };

                customBinding = new CustomBinding(binding);
                customBinding.Elements.Find<HttpTransportBindingElement>().KeepAliveEnabled = false;
            }

            _surveyDataService = new FusionSurveyDataSoapClient(customBinding, new EndpointAddress(url));
        }

        /// <summary>
        /// Releases the service client.
        /// </summary>
        private void ReinitializeClient()
        {
            var surveyDataService = _surveyDataService;

            InitializeClient();

            if (surveyDataService != null)
            {
                surveyDataService.Abort();
            }
        }

        private T DoServiceCall<T>(Func<FusionSurveyDataSoapClient, T> action, string methodName)
        {
            CheckTransaction(methodName);

            try
            {
                return action(_surveyDataService);
            }
            catch (Exception ex)
            {
                ReinitializeClient();

                TraceHelper.TraceException(ex, methodName);

                throw;
            }
        }

        private void CheckTransaction(string methodName)
        {
            if (DatabaseTransactionScope.Current != null)
            {
                Trace.TraceWarning(
                    "Web service method '{0}' is called inside transaction scope '{1}'.",
                    methodName,
                    DatabaseTransactionScope.Current.TransactionName);
            }
        }

        /// <summary>
        /// Returns a <see cref="Confirmit.CATI.Core.SurveyDataService.TransferResult"/>
        /// containing the response data.  All levels in the project will be returned.
        /// </summary>
        /// <param name="transferDef">The transfer definition object. It can be a 
        /// <see cref="Confirmit.CATI.Core.SurveyDataService.SimpleTransferDef"/> or
        /// a <see cref="Confirmit.CATI.Core.SurveyDataService.TransferDef"/> object.</param>
        /// <param name="token">The response token.</param>
        /// <returns>The transfer result. See <see cref="Confirmit.CATI.Core.SurveyDataService.TransferResult"/>
        /// </returns>
        public TransferResult GetData(TransferDefBase transferDef, ResponseToken token)
        {
            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.GetData(clientKey, transferDef, token), "SurveyDataService.GetData");
        }

        /// <summary>
        /// Updates the database with the data in the filled DataSet.
        /// </summary>
        /// <param name="transferDef">TransferDefintion</param>
        /// <param name="ds">DataSet to be Updated.</param>
        /// <param name="applyRules">Indicates if BusinessRules should be applied.</param>
        /// <param name="inTransaction"><b>True</b> runs the update in a transaction, <b>false</b> do not.</param>
        /// <param name="transactionKey">A key, defined by the user to be able to track if the transaction succeeded or not (cannot be a negative number)</param>
        /// <returns>ErrorMessage (Datatype: Array of strings).</returns>
        /// <remarks>If "inTransaction" is set to <b>true</b>, the update will be performed in a transaction. 
        /// If an error occurs, a rollback for the whole update will be executed. 
        /// The "transactionKey" parameter must be specified if "inTransaction" is set to <b>true</b>. 
        /// Since the operation will perform a rollback if an error occurs, 
        /// the return value will not have any function when the a transaction is used.</remarks>
        public ErrorMessage[] UpdateData(TransferDef transferDef, DataSet ds, bool applyRules, bool inTransaction, int transactionKey)
        {
            var clientKey = _confirmitClientKeyProvider.Get();

            return DoServiceCall(x => x.UpdateData(clientKey, transferDef, ds, applyRules, inTransaction, transactionKey), "SurveyDataService.UpdateData");
        }
    }
}