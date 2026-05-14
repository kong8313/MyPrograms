using System;
using Confirmit.CATI.Core.AsynchronousTrigger.MessageProcessors;

namespace Confirmit.CATI.Core.AsynchronousTrigger.MessageProcessors.Fakes
{
    public class StubIMessageProcessorFactory : IMessageProcessorFactory 
    {
        private IMessageProcessorFactory _inner;

        public StubIMessageProcessorFactory()
        {
            _inner = null;
        }

        public IMessageProcessorFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IMessageProcessor CreateStringDelegate(string messageType);
        public CreateStringDelegate CreateString;

        IMessageProcessor IMessageProcessorFactory.Create(string messageType)
        {


            if (CreateString != null)
            {
                return CreateString(messageType);
            } else if (_inner != null)
            {
                return ((IMessageProcessorFactory)_inner).Create(messageType);
            }

            return default(IMessageProcessor);
        }

    }
}