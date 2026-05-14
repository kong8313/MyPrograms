using System;
using System.Threading;
using ConfirmitDialerInterface;
using SimulatorDialerDriver;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class InboundCallsGeneratorThread
    {
        private readonly ILogger _logger;
        private readonly IInboundCalls _inboundCalls;

        private readonly ManualResetEvent _evtStopThread = new ManualResetEvent(false);

        public InboundCallsGeneratorThread(
            ILogger logger,
            IInboundCalls inboundCalls)
        {
            _logger = logger;
            _inboundCalls = inboundCalls;
        }

        private void Execute(
            int companyId,
            int dialerId,
            SimulatorScenario scenario)
        {
            var requestFrequency = scenario.InboundCallRequestFrequency;
            var queueLimit = scenario.InboundCallsQueueLimit;
            
            var call = new InboundCall()
            {
                CompanyId = companyId,
                DialerId = dialerId,
                DdiNumber = scenario.DdiNumber,
                CliNumber = scenario.CliNumber
            };

            _logger.Info("InboundCallsGeneratorThread.Execute", "Started");

            try
            {
                do
                {
                    if (_inboundCalls.Count > queueLimit)
                    {
                        continue;
                    }

                    _inboundCalls.GenerateInboundCall(call);
                } while (!_evtStopThread.WaitOne(requestFrequency, false));
            }
            catch (Exception ex)
            {
                _logger.Info("InboundCallsGeneratorThread.Execute", "Halted: " + ex);
            }
        }

        public void Start(
            int companyId,
            int dialerId,
            SimulatorScenario scenario)
        {
            _evtStopThread.Reset();
            AsyncManager.Execute(_logger, () => Execute(companyId, dialerId, scenario));
        }

        public void Stop()
        {
            _evtStopThread.Set();
        }
    }
}