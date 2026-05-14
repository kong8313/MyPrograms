using System;
using System.Threading;
using Confirmit.CATI.Core.Telephony.IVR.Interfaces;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using System.Collections.Generic;
using ConfirmitDialerInterface;

namespace Confirmit.CATI.Core.Telephony.IVR.Interfaces.Fakes
{
    public class StubIIvrConsoleService : IIvrConsoleService 
    {
        private IIvrConsoleService _inner;

        public StubIIvrConsoleService()
        {
            _inner = null;
        }

        public IIvrConsoleService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ExecutePeriodicalWorkCancellationTokenDelegate(CancellationToken cancellationToken);
        public ExecutePeriodicalWorkCancellationTokenDelegate ExecutePeriodicalWorkCancellationToken;

        void IIvrConsoleService.ExecutePeriodicalWork(CancellationToken cancellationToken)
        {

            if (ExecutePeriodicalWorkCancellationToken != null)
            {
                ExecutePeriodicalWorkCancellationToken(cancellationToken);
            } else if (_inner != null)
            {
                ((IIvrConsoleService)_inner).ExecutePeriodicalWork(cancellationToken);
            }
        }

        public delegate void ProcessCallOnConnectBvTasksEntityDelegate(BvTasksEntity task);
        public ProcessCallOnConnectBvTasksEntityDelegate ProcessCallOnConnectBvTasksEntity;

        void IIvrConsoleService.ProcessCallOnConnect(BvTasksEntity task)
        {

            if (ProcessCallOnConnectBvTasksEntity != null)
            {
                ProcessCallOnConnectBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((IIvrConsoleService)_inner).ProcessCallOnConnect(task);
            }
        }

        public delegate void ProcessIvrSubmitBvTasksEntityInt64ArrayOfKeyValuePairOfStringStringDelegate(BvTasksEntity task, long campaignId, KeyValuePair<string, string>[] variables);
        public ProcessIvrSubmitBvTasksEntityInt64ArrayOfKeyValuePairOfStringStringDelegate ProcessIvrSubmitBvTasksEntityInt64ArrayOfKeyValuePairOfStringString;

        void IIvrConsoleService.ProcessIvrSubmit(BvTasksEntity task, long campaignId, KeyValuePair<string, string>[] variables)
        {

            if (ProcessIvrSubmitBvTasksEntityInt64ArrayOfKeyValuePairOfStringString != null)
            {
                ProcessIvrSubmitBvTasksEntityInt64ArrayOfKeyValuePairOfStringString(task, campaignId, variables);
            } else if (_inner != null)
            {
                ((IIvrConsoleService)_inner).ProcessIvrSubmit(task, campaignId, variables);
            }
        }

        public delegate void ProcessAgentStateBvTasksEntityDelegate(BvTasksEntity task);
        public ProcessAgentStateBvTasksEntityDelegate ProcessAgentStateBvTasksEntity;

        void IIvrConsoleService.ProcessAgentState(BvTasksEntity task)
        {

            if (ProcessAgentStateBvTasksEntity != null)
            {
                ProcessAgentStateBvTasksEntity(task);
            } else if (_inner != null)
            {
                ((IIvrConsoleService)_inner).ProcessAgentState(task);
            }
        }

        public delegate void ProcessTransferStateBvActiveDialEntityStringTransferStateDelegate(BvActiveDialEntity dial, string transferId, TransferState transferState);
        public ProcessTransferStateBvActiveDialEntityStringTransferStateDelegate ProcessTransferStateBvActiveDialEntityStringTransferState;

        void IIvrConsoleService.ProcessTransferState(BvActiveDialEntity dial, string transferId, TransferState transferState)
        {

            if (ProcessTransferStateBvActiveDialEntityStringTransferState != null)
            {
                ProcessTransferStateBvActiveDialEntityStringTransferState(dial, transferId, transferState);
            } else if (_inner != null)
            {
                ((IIvrConsoleService)_inner).ProcessTransferState(dial, transferId, transferState);
            }
        }

    }
}