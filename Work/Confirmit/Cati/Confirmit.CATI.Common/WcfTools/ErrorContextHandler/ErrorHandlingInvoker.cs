using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Common.WcfTools.ConsoleMessageHeader;

namespace Confirmit.CATI.Common.WcfTools.ErrorContextHandler
{
    public class ErrorHandlingInvoker : IOperationInvoker
    {
        private const string AdditionalErrorMessageText = "NOTE: This exception has not been thrown to the client because of [LogExceptionAndNotReThrow] tag applied to the web service method";
        private readonly IOperationInvoker _childInvoker;
        private readonly WebServiceType _webServiceType;

        public ErrorHandlingInvoker(IOperationInvoker childInvoker, WebServiceType webServiceType)
        {
            _childInvoker = childInvoker;
            _webServiceType = webServiceType;
        }

        public object[] AllocateInputs()
        {
            return _childInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            try
            {
                return _childInvoker.Invoke(instance, inputs, out outputs);
            }
            catch (Exception error)
            {
                Exception resultingException = error;

                bool rethrow = LogError(inputs, error);

                if (error is UserMessageException)
                {
                    resultingException = ((UserMessageException)error).ToFault();
                }
                else if (_webServiceType == WebServiceType.External && !(error is FaultException<UserMessageException>))
                {
                    resultingException = new FaultException("Internal server error.", new FaultCode(Constants.InternalServerErrorFaultCode));
                }
                else if (rethrow)
                {
                    throw;
                }

                if (rethrow)
                {
                    throw resultingException;
                }

                outputs = new object[0];
                return null;
            }
        }

        private bool LogError(object[] inputs, Exception error)
        {
            bool rethrow = false;
            try
            {
                var context = CreateErrorContext(inputs);

                rethrow = ShouldRethrowError(context);

                var logger = new ErrorContextLogger();

                if (!rethrow)
                {
                    logger.AdditionalText = AdditionalErrorMessageText;
                }

                logger.LogError(error, context);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error in ErrorHandlingInvoker:\r\n" + ex);
            }

            return rethrow;
        }

        private static bool ShouldRethrowError(ErrorContext context)
        {
            try
            {
                var paramTypes = context.Parameters.Select(x => x?.GetType()).ToArray();

                var methodInfo = context.ServiceType.GetMethod(context.MethodName, paramTypes);

                return (methodInfo == null) ||
                       (methodInfo.GetCustomAttributes(typeof(LogExceptionAndNotReThrowAttribute), true).Length == 0);
            }
            catch (Exception ex)
            {
                string extraInfo;

                if ((context == null) || (context.Parameters == null))
                {
                    extraInfo = (context == null) ? "context=[null]" :
                        (context.Parameters == null) ? "context.Parameters=[null]" : "";
                }
                else
                {
                    extraInfo = "Method [" + context.ServiceType + "."
                                + context.MethodName + "("
                                + string.Join(", ", context.Parameters.Select(x => x?.GetType())) + ")]";
                }

                Trace.TraceWarning("ErrorHandlingInvoker.ShouldRethrowError: " + ex + " /// " + extraInfo);

                return true;
            }
        }

        private ErrorContext CreateErrorContext(object[] inputs)
        {
            var operationContext = OperationContext.Current;
            var serviceDescription = operationContext.InstanceContext.Host.Description;
            var messageHeaders = operationContext.IncomingMessageHeaders;

            var context = new ErrorContext
            {
                Parameters = inputs,
                IdentityName = GetIdentityName(),
                ServiceName = serviceDescription.Name,
                ServiceType = serviceDescription.ServiceType,
                ServiceNamespace = serviceDescription.Namespace
            };

            if (messageHeaders != null)
            {
                context.Action = messageHeaders.Action;
                context.ToHeader = messageHeaders.To.AbsoluteUri;
                context.MethodName = GetMethodName(messageHeaders.Action);
            }

            return context;
        }

        private string GetIdentityName()
        {
            string result = null;
            if (ServiceSecurityContext.Current != null && ServiceSecurityContext.Current.PrimaryIdentity != null)
            {
                result = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            }

            if (string.IsNullOrEmpty(result))
            {
                var authorizationMessageHeaderReader = new AuthorizationMessageHeaderReader(new MessageHeaderAccessor());
                result = authorizationMessageHeaderReader.GetIncomingMessageLogin();
            }

            return result;
        }

        private static string GetMethodName(string action)
        {
            return String.IsNullOrEmpty(action) ? "(empty)" : action.Substring(action.LastIndexOf('/') + 1);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return _childInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return _childInvoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return _childInvoker.IsSynchronous; }
        }
    }
}