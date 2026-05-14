using System;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Fakes
{
    public class StubIMessageReader : IMessageReader 
    {
        private IMessageReader _inner;

        public StubIMessageReader()
        {
            _inner = null;
        }

        public IMessageReader Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CancelDelegate();
        public CancelDelegate Cancel;

        void IMessageReader.Cancel()
        {

            if (Cancel != null)
            {
                Cancel();
            } else if (_inner != null)
            {
                ((IMessageReader)_inner).Cancel();
            }
        }

        public delegate RawMessage ReceiveMessageDelegate();
        public ReceiveMessageDelegate ReceiveMessage;

        RawMessage IMessageReader.ReceiveMessage()
        {


            if (ReceiveMessage != null)
            {
                return ReceiveMessage();
            } else if (_inner != null)
            {
                return ((IMessageReader)_inner).ReceiveMessage();
            }

            return default(RawMessage);
        }

        private int _QueueReadTimeout;
        public Func<int> QueueReadTimeoutGet;
        public Action<int> QueueReadTimeoutSetInt32;

        int IMessageReader.QueueReadTimeout
        {
            get
            {
                if (QueueReadTimeoutGet != null)
                {
                    return QueueReadTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IMessageReader)_inner).QueueReadTimeout;
                }

                if (QueueReadTimeoutSetInt32 == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    return _QueueReadTimeout;
                }

                return default(int);
            }

            set
            {
                if (QueueReadTimeoutSetInt32 != null)
                {
                    QueueReadTimeoutSetInt32(value);
                    return;
                } else if (_inner != null)
                {
                    ((IMessageReader)_inner).QueueReadTimeout = value;
                    return;
                }

                if (QueueReadTimeoutGet == null)
                {
                     // If both setter and getter delegates are not set then implement same way as autoproperty
                    _QueueReadTimeout = value;
                }

            }
        }

    }
}