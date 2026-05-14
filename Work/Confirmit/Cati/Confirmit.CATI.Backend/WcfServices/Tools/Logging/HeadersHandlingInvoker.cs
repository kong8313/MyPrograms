using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Confirmit.CATI.Core.Services;
using Confirmit.Logging;

namespace Confirmit.CATI.Backend.WcfServices.Tools.Logging
{
    public class HeadersHandlingInvoker : IOperationInvoker
    {
        private readonly IOperationInvoker _childInvoker;
        private readonly ThreadIdentityService _threadIdentityService = new ThreadIdentityService();

        public HeadersHandlingInvoker(IOperationInvoker childInvoker)
        {
            _childInvoker = childInvoker;
        }

        public object[] AllocateInputs()
        {
            return _childInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            _threadIdentityService.SetPrincipalForIncomingWcfRequest();
            
            var registeredLoggingFieldsScopes = RegisterLoggingFieldsScopes();
            
            try
            {
                return _childInvoker.Invoke(instance, inputs, out outputs);
            }
            finally
            {
                foreach (var scope in registeredLoggingFieldsScopes)
                {
                    scope?.Dispose();
                }

                _threadIdentityService.ResetPrincipal();
            }
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            _threadIdentityService.SetPrincipalForIncomingWcfRequest();
            return _childInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            _threadIdentityService.ResetPrincipal();
            return _childInvoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous => _childInvoker.IsSynchronous;

        private static IEnumerable<IDisposable> RegisterLoggingFieldsScopes()
        {
            const string headersNamespace = "https://www.confirmit.com/InterviewerApi/WcfMessageHeaderInspector";
            const string correlationIdHeaderName = "X-Confirmit-Correlation-Id";
            const string referrerServiceHeaderName = "X-Confirmit-User-Agent";
            const string initiatingServiceHeaderName = "X-Confirmit-Initiating-Service";

            var disposableObjects = new List<IDisposable>();
            var headers = OperationContext.Current.IncomingMessageHeaders;

            if (headers.FindHeader(correlationIdHeaderName, headersNamespace) != -1)
            {
                var correlationIdHeader = headers.GetHeader<string>(correlationIdHeaderName, headersNamespace);
                disposableObjects.Add(LogFactory.Register(new CustomField("CorrelationId", correlationIdHeader)));
            }

            if (headers.FindHeader(initiatingServiceHeaderName, headersNamespace) != -1)
            {
                var initiatingServiceHeader = headers.GetHeader<string>(initiatingServiceHeaderName, headersNamespace);
                disposableObjects.Add(LogFactory.Register(new CustomField("InitiatingService", initiatingServiceHeader)));
            }

            if (headers.FindHeader(referrerServiceHeaderName, headersNamespace) != -1)
            {
                var referrerServiceHeader = headers.GetHeader<string>(referrerServiceHeaderName, headersNamespace);
                disposableObjects.Add(LogFactory.Register(new CustomField("ReferrerService", referrerServiceHeader)));
            }

            return disposableObjects;
        }
    }
}