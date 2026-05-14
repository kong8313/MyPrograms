using System;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Telephony.Console;

namespace Confirmit.CATI.Core.Telephony.Console.Fakes
{
    public class StubIConsoleTransferProcessProcessor : IConsoleTransferProcessProcessor 
    {
        private IConsoleTransferProcessProcessor _inner;

        public StubIConsoleTransferProcessProcessor()
        {
            _inner = null;
        }

        public IConsoleTransferProcessProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool ShouldProcessTransferBvTasksEntityDelegate(BvTasksEntity task);
        public ShouldProcessTransferBvTasksEntityDelegate ShouldProcessTransferBvTasksEntity;

        bool IConsoleTransferProcessProcessor.ShouldProcessTransfer(BvTasksEntity task)
        {


            if (ShouldProcessTransferBvTasksEntity != null)
            {
                return ShouldProcessTransferBvTasksEntity(task);
            } else if (_inner != null)
            {
                return ((IConsoleTransferProcessProcessor)_inner).ShouldProcessTransfer(task);
            }

            return default(bool);
        }

        public delegate void ProcessTransferBvPersonEntityDelegate(BvPersonEntity person);
        public ProcessTransferBvPersonEntityDelegate ProcessTransferBvPersonEntity;

        void IConsoleTransferProcessProcessor.ProcessTransfer(BvPersonEntity person)
        {

            if (ProcessTransferBvPersonEntity != null)
            {
                ProcessTransferBvPersonEntity(person);
            } else if (_inner != null)
            {
                ((IConsoleTransferProcessProcessor)_inner).ProcessTransfer(person);
            }
        }

    }
}