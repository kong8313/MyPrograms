using System;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;
using Confirmit.CATI.Core.AsynchronousTrigger.MessageProcessors;

namespace Confirmit.CATI.Core.AsynchronousTrigger.MessageProcessors.Fakes
{
    public class StubIMessageProcessor : IMessageProcessor 
    {
        private IMessageProcessor _inner;

        public StubIMessageProcessor()
        {
            _inner = null;
        }

        public IMessageProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ProcessRawMessageDelegate(RawMessage rawMessage);
        public ProcessRawMessageDelegate ProcessRawMessage;

        void IMessageProcessor.Process(RawMessage rawMessage)
        {

            if (ProcessRawMessage != null)
            {
                ProcessRawMessage(rawMessage);
            } else if (_inner != null)
            {
                ((IMessageProcessor)_inner).Process(rawMessage);
            }
        }

    }
}