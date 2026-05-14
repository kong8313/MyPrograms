using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Confirmit.CATI.Common.Contracts.ErrorReportingService;
using Confirmit.CATI.Common.WcfTools;
using Confirmit.CATI.Telephony;
using DialerCommon.TraceListeners;
using System.Linq;

namespace DialerCommon.Logging.TraceListeners
{
    public class WsReportingTraceListener : TraceListener
    {
        [ThreadStatic]
        private static int _companyId;
        
        private const int MaxRetryCount = 3;        
        private const int ThreadSleepTimeout = 1000;
        private const int MaxMessagesCountToSend = 100;

        private readonly Logger _logger;        
        private readonly IErrorSender _sender;

        private readonly ConcurrentQueue<ErrorMessage> _errorsQueue = new ConcurrentQueue<ErrorMessage>();
        
        private readonly ChannelFactoryWrapper<IErrorReportingService> _channelFactoryWrapper;

        public WsReportingTraceListener(IErrorSender sender, Logger logger)
        {
            _sender = sender;
            _logger = logger;

            var confirmitDefaultHostname = Environment.GetEnvironmentVariable("Confirmit__DefaultHostname");
            var errorServiceConfiguration = string.IsNullOrEmpty(confirmitDefaultHostname)
                ? (IChannelFactoryWrapperConfiguration)new ErrorReportingServiceChannelFactoryWrapperConfiguration()
                : new ErrorReportingServiceInternalChannelFactoryWrapperConfiguration();

            var catiCommonILoggerToCodiILogger = new CatiCommonILoggerToCodiILogger(logger);

            _channelFactoryWrapper = new ChannelFactoryWrapper<IErrorReportingService>(
                errorServiceConfiguration,
                catiCommonILoggerToCodiILogger);

            var thread = new Thread(ThreadProc) { IsBackground = true };

            thread.Start();
        }     

        public void SetCompanyId(int id)
        {
            _companyId = id;            
        }
           
        public void Release()
        {
            _channelFactoryWrapper.Release();
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if ((Filter == null) || Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null))
            {
                EnqueueMessage(message);
            }
        }

        public override void WriteLine(string o)
        {
            /* Do nothing */
        }

        public override void Write(string o)
        {
            /* Do nothing */
        }

        private void EnqueueMessage(string message)
        {
            var localTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");

            message += Environment.NewLine + "----------" + Environment.NewLine;
            message += String.Format("Local time: {0}\r\nUUID: {1}", localTime, Guid.NewGuid());

            _errorsQueue.Enqueue(new ErrorMessage(_companyId, message));
        }

        private void ThreadProc(object parameter)
        {
            do
            {
                try
                {
                    SendErrorsToErrorReportingService();
                }
                catch (Exception ex)
                {
                    _logger.WriteErrorToFileTraceListenerOnly(string.Format("Error during error sending thread execution: {0}", ex));
                }

                Thread.Sleep(ThreadSleepTimeout);
            }
            while (true);
        }

        private void SendErrorsToErrorReportingService()
        {       
            bool success = false;

            int retryCount = 0;

            do
            {
                try
                {
                    var messages = GetAvailableMessages();

                    if (messages.Any())
                    {
                        if (retryCount > 0)
                        {
                            messages = UpdateMessageWithRetryInformation(messages, retryCount);
                        }
                        _channelFactoryWrapper.Execute(x => _sender.SendErrorMessages(x, messages));
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    _logger.WriteErrorToFileTraceListenerOnly(String.Format("Exception thrown while loggin error with errors reporting service:\r\nException:\r\n{0}", ex));

                    retryCount++;
                    Thread.Sleep(500);
                }
            }
            while (!success && retryCount < MaxRetryCount);
        }

        private IEnumerable<ErrorMessage> UpdateMessageWithRetryInformation(IEnumerable<ErrorMessage> errorMessages, int retryCount)
        {
            return (from message in errorMessages 
                    let messageText = string.Format("{0}\r\nRetry Count: {1}\r\n", message.Message, retryCount) 
                    select 
                    new ErrorMessage(message.CompanyId, messageText));
        }

        private IEnumerable<ErrorMessage> GetAvailableMessages()
        {
            var result = new List<ErrorMessage>();

            for (var i = 0; i < MaxMessagesCountToSend; i++)
            {
                ErrorMessage item;                

                if (_errorsQueue.TryDequeue(out item))
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
