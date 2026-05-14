using System;
using ConfirmitDialerInterface;
using SimulatorDialerDriver;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class Dialer
    {
        public int CompanyId { get; set; }
        public int DialerId { get; set; }
        public ISimulator Simulator { get; }
        public DateTime InitializationTime { get; set; }

        public InterviewersManager InterviewersManager { get; }
        public CampaignsManager CampaignsManager { get; }
        public CallManager GlobalCallManager { get; }
        public InboundCalls GlobalInboundCalls { get; }
        public Transfers Transfers { get; }
        public InboundDdiNumber[] InboundDdiNumbers { get; set; } = { };

        private InboundCallsGeneratorThread _inboundCallsGeneratorThread;

        public Dialer(int companyId, int dialerId, ISimulator simulator, ILogger logger, IDialerEvents events)
        {
            CompanyId = companyId;
            DialerId = dialerId;
            Simulator = simulator;
            InitializationTime = DateTime.UtcNow;
            InterviewersManager = new InterviewersManager(simulator, this);
            CampaignsManager = new CampaignsManager(simulator, this);
            GlobalCallManager = new CallManager();
            GlobalInboundCalls = new InboundCalls(logger, events);
            Transfers = new Transfers(events, simulator, this, InterviewersManager);

            if (Simulator.Scenario.GenerateInboundCalls)
            {
                Simulator.Logger.Info("SimulatorDialerDriver.Initialize", "GenerateInboundCalls is enabled in scenario.");
                _inboundCallsGeneratorThread = new InboundCallsGeneratorThread(Simulator.Logger, GlobalInboundCalls);
                _inboundCallsGeneratorThread.Start(companyId, dialerId, Simulator.Scenario);
                Simulator.Logger.Info("SimulatorDialerDriver.Initialize", "Inbound calls generator is started");
            }
        }

        public void ValidateState()
        {
            if (Simulator.Scenario.GenerateInboundCalls && (_inboundCallsGeneratorThread == null))
            {
                throw new DialerException(DialerErrorCode.InvalidParameter,
                    string.Format("Already initialized, GenerateInboundCalls is [{0}] but _inboundCallsGeneratorThread is [null].",
                        Simulator.Scenario.GenerateInboundCalls));
            }
        }

        public void Destroy()
        {
            Simulator.Logger.Verbose("Dialer.Destroy", "Stopping _CampaignsManager");
            CampaignsManager.DestroyAll();
            Simulator.Logger.Verbose("Dialer.Destroy", "Stopping _numberRequestGenerator");

            if (_inboundCallsGeneratorThread != null)
            {
                Simulator.Logger.Verbose("Dialer.Destroy", "Stopping _inboundCallsGeneratorThread");
                _inboundCallsGeneratorThread.Stop();
                _inboundCallsGeneratorThread = null;
            }


        }

        public void ConfigureInboundDdiNumbers(InboundDdiNumber[] inboundDdiNumbers)
        {
            InboundDdiNumbers = inboundDdiNumbers;
        }
    }
}