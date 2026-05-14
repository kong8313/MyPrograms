using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleTransferCancelProcessor : IConsoleTransferCancelProcessor 
    {
        private IConsoleTransferCancelProcessor _inner;

        public StubIConsoleTransferCancelProcessor()
        {
            _inner = null;
        }

        public IConsoleTransferCancelProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void TransferCancelBvTasksEntityBvPersonEntityTransferCancelEventBvActiveDialEntityDelegate(BvTasksEntity task, BvPersonEntity person, TransferCancelEvent activityEvent, BvActiveDialEntity activeDial);
        public TransferCancelBvTasksEntityBvPersonEntityTransferCancelEventBvActiveDialEntityDelegate TransferCancelBvTasksEntityBvPersonEntityTransferCancelEventBvActiveDialEntity;

        void IConsoleTransferCancelProcessor.TransferCancel(BvTasksEntity task, BvPersonEntity person, TransferCancelEvent activityEvent, BvActiveDialEntity activeDial)
        {

            if (TransferCancelBvTasksEntityBvPersonEntityTransferCancelEventBvActiveDialEntity != null)
            {
                TransferCancelBvTasksEntityBvPersonEntityTransferCancelEventBvActiveDialEntity(task, person, activityEvent, activeDial);
            } else if (_inner != null)
            {
                ((IConsoleTransferCancelProcessor)_inner).TransferCancel(task, person, activityEvent, activeDial);
            }
        }

    }
}