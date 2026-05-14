using System;
using Confirmit.CATI.Core.AsyncOperations.Framework;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;

namespace Confirmit.CATI.Core.AsyncOperations.Framework.Fakes
{
    public class StubIAsyncOperationQueue : IAsyncOperationQueue 
    {
        private IAsyncOperationQueue _inner;

        public StubIAsyncOperationQueue()
        {
            _inner = null;
        }

        public IAsyncOperationQueue Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate BvAsyncOperationQueueEntity EnqueueInt32StringBooleanIAsyncOperationParametersInt32StringDelegate(int callCenterId, string title, bool isInitiatedBySystem, IAsyncOperationParameters parameters, int priority, string supervisorName);
        public EnqueueInt32StringBooleanIAsyncOperationParametersInt32StringDelegate EnqueueInt32StringBooleanIAsyncOperationParametersInt32String;

        BvAsyncOperationQueueEntity IAsyncOperationQueue.Enqueue(int callCenterId, string title, bool isInitiatedBySystem, IAsyncOperationParameters parameters, int priority, string supervisorName)
        {


            if (EnqueueInt32StringBooleanIAsyncOperationParametersInt32String != null)
            {
                return EnqueueInt32StringBooleanIAsyncOperationParametersInt32String(callCenterId, title, isInitiatedBySystem, parameters, priority, supervisorName);
            } else if (_inner != null)
            {
                return ((IAsyncOperationQueue)_inner).Enqueue(callCenterId, title, isInitiatedBySystem, parameters, priority, supervisorName);
            }

            return default(BvAsyncOperationQueueEntity);
        }

        public delegate BvAsyncOperationQueueEntity DequeueDelegate();
        public DequeueDelegate Dequeue;

        BvAsyncOperationQueueEntity IAsyncOperationQueue.Dequeue()
        {


            if (Dequeue != null)
            {
                return Dequeue();
            } else if (_inner != null)
            {
                return ((IAsyncOperationQueue)_inner).Dequeue();
            }

            return default(BvAsyncOperationQueueEntity);
        }

        public delegate void UpdateHangedDelegate();
        public UpdateHangedDelegate UpdateHanged;

        void IAsyncOperationQueue.UpdateHanged()
        {

            if (UpdateHanged != null)
            {
                UpdateHanged();
            } else if (_inner != null)
            {
                ((IAsyncOperationQueue)_inner).UpdateHanged();
            }
        }

        public delegate void AbortInt32StringDelegate(int id, string supervisorName);
        public AbortInt32StringDelegate AbortInt32String;

        void IAsyncOperationQueue.Abort(int id, string supervisorName)
        {

            if (AbortInt32String != null)
            {
                AbortInt32String(id, supervisorName);
            } else if (_inner != null)
            {
                ((IAsyncOperationQueue)_inner).Abort(id, supervisorName);
            }
        }

    }
}