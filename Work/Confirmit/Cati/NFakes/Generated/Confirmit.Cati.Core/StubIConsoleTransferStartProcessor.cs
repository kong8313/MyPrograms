using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Common.ConsoleService.Abstract;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleTransferStartProcessor : IConsoleTransferStartProcessor 
    {
        private IConsoleTransferStartProcessor _inner;

        public StubIConsoleTransferStartProcessor()
        {
            _inner = null;
        }

        public IConsoleTransferStartProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void TransferStartBvTasksEntityBvPersonEntityTransferOptionsTransferStartEventDelegate(BvTasksEntity task, BvPersonEntity person, TransferOptions options, TransferStartEvent activityEvent);
        public TransferStartBvTasksEntityBvPersonEntityTransferOptionsTransferStartEventDelegate TransferStartBvTasksEntityBvPersonEntityTransferOptionsTransferStartEvent;

        void IConsoleTransferStartProcessor.TransferStart(BvTasksEntity task, BvPersonEntity person, TransferOptions options, TransferStartEvent activityEvent)
        {

            if (TransferStartBvTasksEntityBvPersonEntityTransferOptionsTransferStartEvent != null)
            {
                TransferStartBvTasksEntityBvPersonEntityTransferOptionsTransferStartEvent(task, person, options, activityEvent);
            } else if (_inner != null)
            {
                ((IConsoleTransferStartProcessor)_inner).TransferStart(task, person, options, activityEvent);
            }
        }

    }
}