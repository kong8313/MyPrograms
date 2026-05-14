using System;
using Confirmit.CATI.Core.AsynchronousTrigger;
using Confirmit.CATI.Core.AsynchronousTrigger.Messages;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Fakes
{
    public class StubIQueueProcessor : IQueueProcessor 
    {
        private IQueueProcessor _inner;

        public StubIQueueProcessor()
        {
            _inner = null;
        }

        public IQueueProcessor Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ReadAndProcessAllMessagesInTheQueueDelegate();
        public ReadAndProcessAllMessagesInTheQueueDelegate ReadAndProcessAllMessagesInTheQueue;

        void IQueueProcessor.ReadAndProcessAllMessagesInTheQueue()
        {

            if (ReadAndProcessAllMessagesInTheQueue != null)
            {
                ReadAndProcessAllMessagesInTheQueue();
            } else if (_inner != null)
            {
                ((IQueueProcessor)_inner).ReadAndProcessAllMessagesInTheQueue();
            }
        }

        public delegate RawMessage ReadAndProcessSingleMessageDelegate();
        public ReadAndProcessSingleMessageDelegate ReadAndProcessSingleMessage;

        RawMessage IQueueProcessor.ReadAndProcessSingleMessage()
        {


            if (ReadAndProcessSingleMessage != null)
            {
                return ReadAndProcessSingleMessage();
            } else if (_inner != null)
            {
                return ((IQueueProcessor)_inner).ReadAndProcessSingleMessage();
            }

            return default(RawMessage);
        }

        public delegate void StopDelegate();
        public StopDelegate Stop;

        void IQueueProcessor.Stop()
        {

            if (Stop != null)
            {
                Stop();
            } else if (_inner != null)
            {
                ((IQueueProcessor)_inner).Stop();
            }
        }

        private int _QueueReadTimeout;
        public Func<int> QueueReadTimeoutGet;
        public Action<int> QueueReadTimeoutSetInt32;

        int IQueueProcessor.QueueReadTimeout
        {
            get
            {
                if (QueueReadTimeoutGet != null)
                {
                    return QueueReadTimeoutGet();
                } else if (_inner != null)
                {
                    return ((IQueueProcessor)_inner).QueueReadTimeout;
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
                    ((IQueueProcessor)_inner).QueueReadTimeout = value;
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