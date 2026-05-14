using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;

using Confirmit.CATI.Common.Exceptions;
using Confirmit.CATI.Common.Logging;

namespace Confirmit.CATI.Common.WcfTools
{
    /// <summary>
    /// Represents class which executes WCF services methods and logs
    /// execution time if it is needed.
    /// </summary>
    public class WcfExecutor
    {
        private readonly bool enableVerboseLogging;

        private readonly bool logExceptions;

        private readonly ILogger logger;

        /// <summary>
        /// Timeout (in seconds) that specifies how long the close operation has to complete before timing out.
        /// </summary>
        private const int ClientProxyCloseTimeout = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfExecutor"/> class.
        /// </summary>
        /// <param name="enableVerboseLogging">if set to <c>true</c> verbose logging of method calls and their timings is enabled.</param>
        /// <param name="logExceptions">if set to <c>true</c> all exceptions occurred during WCF method calls will be logged.</param>
        /// <param name="logger">The logger.</param>
        public WcfExecutor(bool enableVerboseLogging, bool logExceptions, ILogger logger)
        {
            this.enableVerboseLogging = enableVerboseLogging;
            this.logExceptions = logExceptions;
            this.logger = logger;
        }

        /// <summary>
        /// Executes method of WCF service TService and logs execution time and any thrown exceptions.
        /// Service client instance is provided by factory method "instanceProvider".
        /// This function doesn't modify state of provided client instance.
        /// Method for execution is provided as lambda expression and gives
        /// us ability to get name of the method.
        /// </summary>
        /// <typeparam name="TService">WCF service interface.</typeparam>
        /// <param name="instanceProvider">Factory method which provides us instance of TService service client.</param>
        /// <param name="method">Method call delegate.</param>
        /// <param name="methodName">The name of the method being called.</param>
        public void Execute<TService>(Func<TService> instanceProvider, Action<TService> method, string methodName)
        {
            LogAndRethrow(
                () => Execute(instanceProvider, method, (service, action) => action(service), methodName),
                GetServiceAndMethodName<TService>(methodName));
        }

        /// <summary>
        /// Executes method of WCF service TService and logs execution time and any thrown exceptions.
        /// Service client instance is provided by factory method "instanceProvider".
        /// This function opens provided client instance and closes it after work.
        /// Method for execution is provided as lambda expression and gives
        /// us ability to get name of the method.
        /// </summary>
        /// <typeparam name="TService">WCF service interface.</typeparam>
        /// <param name="instanceProvider">Factory method which provides us instance of TService service client.</param>
        /// <param name="method">Method call delegate.</param>
        /// <param name="methodName">The name of the method being called.</param>
        public void ExecuteAndRelease<TService>(Func<TService> instanceProvider, Action<TService> method, string methodName)
        {
            LogAndRethrow(
                () => Execute(instanceProvider, method, CallMethodAndReleaseClient, methodName),
                GetServiceAndMethodName<TService>(methodName));
        }

        /// <summary>
        /// Executes method of WCF service TService, returns result and logs execution time and any thrown exceptions.
        /// Service client instance is provided by factory method "instanceProvider".
        /// This function doesn't modify state of provided client instance.
        /// Method for execution is provided as lambda expression and gives
        /// us ability to get name of the method.
        /// </summary>
        /// <typeparam name="TService">WCF service interface.</typeparam>
        /// <typeparam name="TResult">Function result type.</typeparam>
        /// <param name="instanceProvider">Factory method which provides us instance of TService service client.</param>
        /// <param name="method">Method call delegate.</param>
        /// <param name="methodName">The name of the method being called.</param>
        public TResult Execute<TService, TResult>(Func<TService> instanceProvider, Func<TService, TResult> method, string methodName)
        {
            return LogAndRethrow(
                () => Execute(instanceProvider, method, (service, function) => function(service), methodName),
                GetServiceAndMethodName<TService>(methodName));
        }

        /// <summary>
        /// Executes method of WCF service TService, returns result and logs execution time and any thrown exceptions.
        /// Service client instance is provided by factory method "instanceProvider".
        /// This function opens provided client instance and closes it after work.
        /// Method for execution is provided as lambda expression and gives
        /// us ability to get name of the method.
        /// </summary>
        /// <typeparam name="TService">WCF service interface.</typeparam>
        /// <typeparam name="TResult">Function result type.</typeparam>
        /// <param name="instanceProvider">Factory method which provides us instance of TService service client.</param>
        /// <param name="method">Method call delegate.</param>
        /// <param name="methodName">The name of the method being called.</param>
        public TResult ExecuteAndRelease<TService, TResult>(Func<TService> instanceProvider, Func<TService, TResult> method, string methodName)
        {
            return LogAndRethrow(
                () => Execute(instanceProvider, method, CallMethodAndReleaseClient, methodName),
                GetServiceAndMethodName<TService>(methodName));
        }

        private void LogAndRethrow(Action action, string serviceAndMethodName)
        {
            LogAndRethrow(action.WrapInFunc<bool>(), serviceAndMethodName);
        }

        private T LogAndRethrow<T>(Func<T> action, string serviceAndMethodName)
        {
            try
            {
                return action();
            }
            catch (FaultException ex)
            {
                UserMessageException userException;
                if (IsUserFaultException(ex, out userException))
                {
                    LogError(ex, TraceEventType.Warning, serviceAndMethodName);
                    throw userException;
                }

                LogError(ex, TraceEventType.Error, serviceAndMethodName);
                throw;
            }
            catch (Exception ex)
            {
                LogError(ex, TraceEventType.Error, serviceAndMethodName);
                throw;
            }
        }

        private void LogError(Exception ex, TraceEventType severity = TraceEventType.Error, string serviceAndMethodName = null)
        {
            if (logExceptions)
            {
                var serviceAndMethodNameInfo = string.IsNullOrEmpty(serviceAndMethodName)
                    ? ""
                    : serviceAndMethodName + " failed:\r\n";
                
                logger.Log(serviceAndMethodNameInfo + ex, severity);
            }
        }

        public static string GetServiceAndMethodName<TService>(string methodName)
        {
            return $"{typeof(TService).Name}.{methodName}";
        }

        /// <summary>
        /// Executes method of WCF service TService and logs execution time.
        /// Service client instance is provided by factory method "instanceProvider".
        /// Method for execution is provided as lambda expression and gives
        /// us ability to get name of the method.
        /// Action executor is provided with methodCaller function.
        /// </summary>
        /// <typeparam name="TService">WCF service interface.</typeparam>
        /// <param name="instanceProvider">Factory method which provides us instance of TService service client.</param>
        /// <param name="method">Method call delegate.</param>
        /// <param name="methodCaller">Method which calls given action.</param>
        /// <param name="methodName">The name of the method being called.</param>
        private void Execute<TService>(
            Func<TService> instanceProvider,
            Action<TService> method,
            Action<TService, Action<TService>> methodCaller,
            string methodName)
        {
            var serviceAndMethodName = GetServiceAndMethodName<TService>(methodName);

            EventDetailsScope.Current.AddTiming("Start WCF method execution:" + serviceAndMethodName);

            TService service = instanceProvider();

            EventDetailsScope.Current.AddTiming("WCF method call expression compiled");

            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                methodCaller(service, method);
            }
            finally
            {
                if (enableVerboseLogging)
                {
                    timer.Stop();
                    logger.Log(
                        string.Format("Method {0} finished in {1} seconds", serviceAndMethodName, timer.Elapsed),
                        TraceEventType.Verbose);
                }
            }

            EventDetailsScope.Current.AddTiming("WCF method call completed");
        }

        /// <summary>
        /// Executes method of WCF service TService, returns result and logs execution time.
        /// Service client instance is provided by factory method "instanceProvider".
        /// Method for execution is provided as lambda expression and gives
        /// us ability to get name of the method.
        /// Action executor is provided with methodCaller function.
        /// </summary>
        /// <typeparam name="TService">WCF service interface.</typeparam>
        /// <typeparam name="TResult">Function result type.</typeparam>
        /// <param name="instanceProvider">Factory method which provides us instance of TService service client.</param>
        /// <param name="method">Method call delegate.</param>
        /// <param name="methodCaller">Method which calls given action.</param>
        /// <param name="methodName">The name of the method being called.</param>
        private TResult Execute<TService, TResult>(
            Func<TService> instanceProvider,
            Func<TService, TResult> method,
            Func<TService, Func<TService, TResult>, TResult> methodCaller,
            string methodName)
        {
            var serviceAndMethodName = GetServiceAndMethodName<TService>(methodName);

            EventDetailsScope.Current.AddTiming("Start WCF method execution:" + serviceAndMethodName);

            TService service = instanceProvider();

            EventDetailsScope.Current.AddTiming("WCF method call expression compiled");

            TResult result;

            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                result = methodCaller(service, method);
            }
            finally
            {
                if (enableVerboseLogging)
                {
                    timer.Stop();
                    logger.Log(
                        string.Format("Method {0} finished in {1} seconds", serviceAndMethodName, timer.Elapsed),
                        TraceEventType.Verbose);
                }
            }

            EventDetailsScope.Current.AddTiming("WCF method call completed");
            return result;
        }

        /// <summary>
        /// Opens given instance of TService service client, calls given action and releases 
        /// service client.
        /// </summary>
        /// <typeparam name="TService">WCF service interface.</typeparam>
        /// <param name="clientInstance">Service client instance.</param>
        /// <param name="action">Service method.</param>
        private void CallMethodAndReleaseClient<TService>(TService clientInstance, Action<TService> action)
        {
            CallMethodAndReleaseClient(clientInstance, action.WrapInFunc<TService, bool>());
        }

        /// <summary>
        /// Opens given instance of TService service client, calls given action and releases 
        /// service client.
        /// </summary>
        /// <typeparam name="TService">WCF service interface.</typeparam>
        /// <typeparam name="TResult">Function result type.</typeparam>
        /// <param name="clientInstance">Service client instance.</param>
        /// <param name="function">Service method.</param>
        private TResult CallMethodAndReleaseClient<TService, TResult>(TService clientInstance, Func<TService, TResult> function)
        {
            var proxy = (IClientChannel)clientInstance;

            try
            {
                EventDetailsScope.Current.AddTiming("Opening WCF channel");
                proxy.Open();
                EventDetailsScope.Current.AddTiming("WCF channel is opened");

                TResult result = function(clientInstance);

                EventDetailsScope.Current.AddTiming("Method called");

                return result;
            }
            finally
            {
                ReleaseCommunicationObject(proxy);
            }
        }

        /// <summary>
        /// Releases WCF communication object.
        /// </summary>
        public void ReleaseCommunicationObject(ICommunicationObject communicationObject)
        {
            if (communicationObject != null)
            {
                try
                {
                    EventDetailsScope.Current.AddTiming("Closing WCF channel");
                    communicationObject.Close(TimeSpan.FromSeconds(ClientProxyCloseTimeout));
                    EventDetailsScope.Current.AddTiming("WCF channel closed");
                }
                catch (CommunicationException ex)
                {
                    LogError(ex);
                    communicationObject.Abort();
                    EventDetailsScope.Current.AddTiming("WCF channel aborted");
                }
                catch (TimeoutException ex)
                {
                    LogError(ex);
                    communicationObject.Abort();
                    EventDetailsScope.Current.AddTiming("WCF channel aborted");
                }
                catch (Exception ex)
                {
                    LogError(ex);
                    communicationObject.Abort();
                    EventDetailsScope.Current.AddTiming("WCF channel aborted");
                    throw;
                }

            }
        }

        /// <summary>
        /// Determines whether the specified exception is user fault exception.
        /// </summary>
        /// <param name="ex">The <see cref="FaultException"/> to check.</param>
        /// <param name="userException"><see cref="UserMessageException"/> or derived from 
        /// <see cref="UserMessageException"/> extracted from input <see cref="FaultException"/>.</param>
        /// <returns>
        /// <c>true</c> if specified exception is user fault exception; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsUserFaultException(FaultException ex, out UserMessageException userException)
        {
            userException = null;
            if (ex.GetType().IsGenericType)
            {
                var genericArguments = ex.GetType().GetGenericArguments();
                if (genericArguments.Count() == 1)
                {
                    Type detailsType = genericArguments.Single();
                    if (detailsType == typeof(UserMessageExceptionDetails) ||
                        detailsType.IsSubclassOf(typeof(UserMessageExceptionDetails)))
                    {
                        var details = (UserMessageExceptionDetails)ex.GetType().GetProperty("Detail").GetValue(ex, null);
                        userException = details.ToException();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}