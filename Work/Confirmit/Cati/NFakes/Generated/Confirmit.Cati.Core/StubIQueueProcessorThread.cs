using System;
using Confirmit.CATI.Core.AsynchronousTrigger;

namespace Confirmit.CATI.Core.AsynchronousTrigger.Fakes
{
    public class StubIQueueProcessorThread : IQueueProcessorThread 
    {
        private IQueueProcessorThread _inner;

        public StubIQueueProcessorThread()
        {
            _inner = null;
        }

        public IQueueProcessorThread Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void ReadAndProcessMessagesLoopDelegate();
        public ReadAndProcessMessagesLoopDelegate ReadAndProcessMessagesLoop;

        void IQueueProcessorThread.ReadAndProcessMessagesLoop()
        {

            if (ReadAndProcessMessagesLoop != null)
            {
                ReadAndProcessMessagesLoop();
            } else if (_inner != null)
            {
                ((IQueueProcessorThread)_inner).ReadAndProcessMessagesLoop();
            }
        }

        public delegate void StartDelegate();
        public StartDelegate Start;

        void IQueueProcessorThread.Start()
        {

            if (Start != null)
            {
                Start();
            } else if (_inner != null)
            {
                ((IQueueProcessorThread)_inner).Start();
            }
        }

        public delegate void StopDelegate();
        public StopDelegate Stop;

        void IQueueProcessorThread.Stop()
        {

            if (Stop != null)
            {
                Stop();
            } else if (_inner != null)
            {
                ((IQueueProcessorThread)_inner).Stop();
            }
        }

    }
}