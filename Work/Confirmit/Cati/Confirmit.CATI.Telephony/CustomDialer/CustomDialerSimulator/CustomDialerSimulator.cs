using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Confirmit.CATI.Console.LightweightTelephony;

namespace CustomDialerSimulator
{
    public class CustomDialerSimulator : ICustomDialer
    {        
        private long _dialCommandIndex;

        public event EventHandler<CallStatusChangedEventArgs> CallStatusChanged;

        private readonly Logger _logger;
        private readonly CommandSettingsProvider _commandSettingsProvider;

        public CustomDialerSimulator()
        {
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _commandSettingsProvider = new CommandSettingsProvider(Path.Combine(directoryPath, "Config.xml"));
            _logger = new Logger(Path.Combine(directoryPath, "CustomDialerSimulator.txt"));
        }

        public void Dial(string phoneNumber)
        {
            _logger.WriteLine("Start dialing " + phoneNumber);            
            ThreadPool.QueueUserWorkItem(DialingProcess);
        }

        private void DialingProcess(object state)
        {
            var commandSettings = _commandSettingsProvider.GetByIndexCyclically(_dialCommandIndex++);

            Thread.Sleep(commandSettings.Timeout);

            var callOutcome = (CustomCallOutcome)(commandSettings.Result);

            _logger.WriteLine("Call outcome: " + callOutcome);

            RaiseCallStatusChangedEvent(callOutcome);
        }

        private void RaiseCallStatusChangedEvent(CustomCallOutcome callOutcome)
        {
            if (CallStatusChanged != null)
            {
                CallStatusChanged(this, new CallStatusChangedEventArgs {CustomCallStatus = callOutcome});
            }
        }

        public void HangUp()
        {
            _logger.WriteLine("HangUp");
        }
    }
}