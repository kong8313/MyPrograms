using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleTransferCompleteProcessor : IConsoleTransferCompleteProcessor 
    {
        private IConsoleTransferCompleteProcessor _inner;

        public StubIConsoleTransferCompleteProcessor()
        {
            _inner = null;
        }

        public IConsoleTransferCompleteProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void TransferCompleteBvTasksEntityBvPersonEntityTransferCompleteEventDelegate(BvTasksEntity task, BvPersonEntity person, TransferCompleteEvent activityEvent);
        public TransferCompleteBvTasksEntityBvPersonEntityTransferCompleteEventDelegate TransferCompleteBvTasksEntityBvPersonEntityTransferCompleteEvent;

        void IConsoleTransferCompleteProcessor.TransferComplete(BvTasksEntity task, BvPersonEntity person, TransferCompleteEvent activityEvent)
        {

            if (TransferCompleteBvTasksEntityBvPersonEntityTransferCompleteEvent != null)
            {
                TransferCompleteBvTasksEntityBvPersonEntityTransferCompleteEvent(task, person, activityEvent);
            } else if (_inner != null)
            {
                ((IConsoleTransferCompleteProcessor)_inner).TransferComplete(task, person, activityEvent);
            }
        }

    }
}