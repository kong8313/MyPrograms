using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ConfirmitDialerInterface;
using SimulatorDialerDriver;
using SimulatorDialerDriver.Controllers;
using SimulatorDialerDriver.Distribution;
using SimulatorDialerDriver.SurveyInstances;

namespace Confirmit.CATI.Telephony.SimulatorDialerDriver
{
    public class Transfers
    {
        private readonly IDialerEvents _dialerEvents;
        private readonly ISimulator _simulator;
        private readonly Dialer _dialer;
        private readonly InterviewersManager _interviewersManager;

        private readonly ConcurrentDictionary<string, Transfer> _transfers;

        public Transfers(IDialerEvents dialerEvents, ISimulator simulator, Dialer dialer, InterviewersManager interviewersManager)
        {
            _dialerEvents = dialerEvents;
            _simulator = simulator;
            _dialer = dialer;
            _interviewersManager = interviewersManager;
            _transfers = new ConcurrentDictionary<string, Transfer>();
        }

        public IEnumerable<Transfer> GetAll() {  return _transfers.Values; }

        public void Start(int companyId, int dialerId, long campaignId, string transferId, int agentId, TransferType transferType)
        {
            var interviewer = _interviewersManager.Get(agentId);

            if (interviewer.ActiveCall == null)
            {
                throw new DialerException($"Can't start transfer, because agentId={agentId} doesn't have active call.");
            }

            var transfer = new Transfer
            {
                CompanyId = companyId,
                DialerId = dialerId,
                CampaignId = campaignId,
                TransferId = transferId,
                InitiatorAgentId = agentId,
                TransferType = transferType,
                Call = new CallManager.CallInfoEx(interviewer.ActiveCall.Info, interviewer.ActiveCall.CampaignId, CallManager.CallType.Transfer),
                TransferState = new TransferState
                {
                    InitiatorAgentId = agentId,
                    InitiatorState = InitiatorState.Connected,
                    ConnectionState = ConnectionState.InitiatorToTarget,
                    TargetState = TargetState.NotDefined,
                    TargetType = TargetType.NotDefined,
                    TargetOutcome = TargetOutcome.NotDefined,
                    TargetResource = null
                }
            };

            if (!_transfers.TryAdd(transferId, transfer))
            {
                throw new DialerException($"Transfer already exists: {transferId}");
            }

            _dialerEvents.NotifyTransferState(
                transfer.CompanyId,
                transfer.DialerId,
                transferId,
                transfer.TransferState);
        }

        public void SetTarget(int companyId, int dialerId, string transferId, TargetType targetType, string targetResource, bool borrowAgentsFromAllCampaigns)
        {
            if (!_transfers.TryGetValue(transferId, out var transfer))
            {
                throw new DialerException($"Transfer is not found: {transferId}");
            }

            transfer.TransferState.TargetType = targetType;
            transfer.TransferState.TargetResource = targetResource;

            switch (targetType)
            {
                case TargetType.Agent:
                    int agentId = int.Parse(targetResource);
                    _interviewersManager.Get(agentId).SendNumberToAgent(transfer.CompanyId, transfer.DialerId, transfer.Call);
                    transfer.TargetAgentId = agentId;
                    transfer.TransferState.TargetOutcome = TargetOutcome.Connected;
                    transfer.TransferState.TargetState = TargetState.Connected;
                    break;

                case TargetType.External:
                    transfer.TransferState.TargetOutcome = TargetOutcome.NotDefined;
                    transfer.TransferState.TargetState = TargetState.Dialing;

                    // TODO: Not the best way to do that
                    new Task(() =>
                    {
                        var interviewer = _interviewersManager.Get(transfer.InitiatorAgentId).Interviewer;
                        var context = new ContextInfo(interviewer, transfer.Call?.Info.interviewId);
                        var outcomeDelay = Generators.ExternalTransferOutcomeDelay.GetValue(context, TimeSpan.FromSeconds(3));

                        Task.Delay(outcomeDelay).Wait();

                        if (!_transfers.ContainsKey(transferId))
                        {
                            return;
                        }

                        transfer.TransferState.TargetOutcome = Generators.ExternalTransferOutcomeValue.GetValue(context, TargetOutcome.Connected);
                        transfer.TransferState.TargetState = transfer.TransferState.TargetOutcome == TargetOutcome.Connected 
                            ? TargetState.Connected
                            : TargetState.NotConnected;

                        _dialerEvents.NotifyTransferState(
                            transfer.CompanyId,
                            transfer.DialerId,
                            transferId,
                            transfer.TransferState);
                    }).Start();

                    break;

                case TargetType.AgentGroup:
                    transfer.TransferState.TargetState = TargetState.WaitingForAgent;

                    QueueTransferCall(transfer, int.Parse(targetResource), borrowAgentsFromAllCampaigns);

                    break;

                default:
                    throw new DialerException($"Unknown TargetType: {targetType} /// transferId={transferId}");
            }

            _dialerEvents.NotifyTransferState(
                transfer.CompanyId,
                transfer.DialerId,
                transferId,
                transfer.TransferState);
        }

        private void QueueTransferCall(Transfer transfer, int groupId, bool borrowAgentsFromAllCampaigns)
        {
            var callInfo = new CallInfo
            {
                agentGroupId = groupId,
                callId = transfer.Call.Info.callId,
                interviewId = transfer.Call.Info.interviewId,
                diallingMode = DialingMode.Preview
            };

            if(!borrowAgentsFromAllCampaigns)
            {
                var campaignController = (CampaignControllerPredictive)_dialer.CampaignsManager.Get(transfer.CampaignId);
                campaignController.CallManager.AddTranferCall(transfer.CampaignId, new []{ transfer.CampaignId }, callInfo);
            }
            else
            {
                _dialer.GlobalCallManager.AddTranferCall(transfer.CampaignId, new[] { transfer.CampaignId }, callInfo);
            }
        }

        public void SetConnectionState(string transferId, ConnectionState state)
        {
            if (!_transfers.ContainsKey(transferId))
            {
                throw new DialerException($"Transfer is not found: {transferId}");
            }

            // Simulator does not support audio atm. So the only thing to do is sending notification.

            var transfer = _transfers[transferId];

            transfer.TransferState.ConnectionState = state;

            _dialerEvents.NotifyTransferState(
                transfer.CompanyId,
                transfer.DialerId,
                transferId,
                transfer.TransferState);
        }

        public void TransferComplete(string transferId)
        {
            if (!_transfers.TryRemove(transferId, out _))
            {
                throw new DialerException($"Transfer is not found: {transferId}");
            }
        }

        public void TransferCancel(string transferId)
        {
            if (!_transfers.TryRemove(transferId, out var transfer))
            {
                throw new DialerException($"Transfer is not found: {transferId}");
            }

            var campaignController = _dialer.CampaignsManager.Get(transfer.CampaignId);
            if (campaignController is CampaignControllerPredictive predictive)
            {
                predictive.CallManager.RemoveCallByIdIfExists(transfer.Call.Info.callId);
                _dialer.GlobalCallManager.RemoveCallByIdIfExists(transfer.Call.Info.callId);
            }
        }

        public void OnCallCompleted(int companyId, int dialerId, long campaignId, IInterviewerController interviewer)
        {
            var callId = interviewer.ActiveCall.Info.callId;
            var agentId = interviewer.Interviewer.AgentId;

            var transfer = _transfers.Values.SingleOrDefault(x => x.Call.Info.callId == callId && x.InitiatorAgentId == agentId);
            if (transfer != null)
            {
                transfer.TransferState.InitiatorState = InitiatorState.NotConnected;
                _dialerEvents.NotifyTransferState(
                    transfer.CompanyId,
                    transfer.DialerId,
                    transfer.TransferId,
                    transfer.TransferState);
            }

            transfer = _transfers.Values.SingleOrDefault(x => x.Call.Info.callId == callId && x.TargetAgentId == agentId);
            if (transfer != null)
            {
                transfer.TransferState.TargetState = TargetState.NotConnected;
                _dialerEvents.NotifyTransferState(
                    transfer.CompanyId,
                    transfer.DialerId,
                    transfer.TransferId,
                    transfer.TransferState);
            }
        }

        public void OnCallConnected(CallManager.CallInfoEx call, IInterviewerController interviewer)
        {
            var callId = call.Info.callId;
            var agentId = interviewer.Interviewer.AgentId;

            var transfer = _transfers.Values.SingleOrDefault(x => x.Call.Info.callId == callId && x.InitiatorAgentId == agentId);
            if (transfer != null)
            {
                transfer.TransferState.InitiatorState = InitiatorState.Connected;
                _dialerEvents.NotifyTransferState(
                    transfer.CompanyId,
                    transfer.DialerId,
                    transfer.TransferId,
                    transfer.TransferState);
            }

            transfer = _transfers.Values.SingleOrDefault(x => x.Call.Info.callId == callId && x.TargetAgentId == agentId);
            if (transfer != null)
            {
                transfer.TransferState.TargetState = TargetState.Connected;
                _dialerEvents.NotifyTransferState(
                    transfer.CompanyId,
                    transfer.DialerId,
                    transfer.TransferId,
                    transfer.TransferState);
            }

        }

        public void AssignCallOnInterviewer(CallManager.CallInfoEx call, InterviewerPredictiveController interviewer)
        {
            var transfer = _transfers.Values.SingleOrDefault(x => x.Call.Info.callId == call.Info.callId);
            if (transfer != null)
            {
                transfer.TargetAgentId = interviewer.Interviewer.AgentId;
                transfer.TransferState.TargetResource = interviewer.Interviewer.AgentId.ToString();
                transfer.TransferState.TargetType = TargetType.Agent;
                transfer.TransferState.TargetState = TargetState.Dialing;
                _dialerEvents.NotifyTransferState(
                    transfer.CompanyId,
                    transfer.DialerId,
                    transfer.TransferId,
                    transfer.TransferState);
            }
        }
    }
}