using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleTransferSetConnectionStateProcessor : IConsoleTransferSetConnectionStateProcessor 
    {
        private IConsoleTransferSetConnectionStateProcessor _inner;

        public StubIConsoleTransferSetConnectionStateProcessor()
        {
            _inner = null;
        }

        public IConsoleTransferSetConnectionStateProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void TransferSetConnectionStateBvTasksEntityBvPersonEntityTransferConnectionStateTransferSetConnectionStateEventDelegate(BvTasksEntity task, BvPersonEntity person, TransferConnectionState transferConnectionState, TransferSetConnectionStateEvent activityEvent);
        public TransferSetConnectionStateBvTasksEntityBvPersonEntityTransferConnectionStateTransferSetConnectionStateEventDelegate TransferSetConnectionStateBvTasksEntityBvPersonEntityTransferConnectionStateTransferSetConnectionStateEvent;

        void IConsoleTransferSetConnectionStateProcessor.TransferSetConnectionState(BvTasksEntity task, BvPersonEntity person, TransferConnectionState transferConnectionState, TransferSetConnectionStateEvent activityEvent)
        {

            if (TransferSetConnectionStateBvTasksEntityBvPersonEntityTransferConnectionStateTransferSetConnectionStateEvent != null)
            {
                TransferSetConnectionStateBvTasksEntityBvPersonEntityTransferConnectionStateTransferSetConnectionStateEvent(task, person, transferConnectionState, activityEvent);
            } else if (_inner != null)
            {
                ((IConsoleTransferSetConnectionStateProcessor)_inner).TransferSetConnectionState(task, person, transferConnectionState, activityEvent);
            }
        }

    }
}