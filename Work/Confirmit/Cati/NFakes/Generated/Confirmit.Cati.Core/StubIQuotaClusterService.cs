using System;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;

namespace Confirmit.CATI.Core.Services.CallDelivery.Interfaces.Fakes
{
    public class StubIQuotaClusterService : IQuotaClusterService 
    {
        private IQuotaClusterService _inner;

        public StubIQuotaClusterService()
        {
            _inner = null;
        }

        public IQuotaClusterService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate bool TryIncrenentInt32Int32Delegate(int surveyId, int callId);
        public TryIncrenentInt32Int32Delegate TryIncrenentInt32Int32;

        bool IQuotaClusterService.TryIncrenent(int surveyId, int callId)
        {


            if (TryIncrenentInt32Int32 != null)
            {
                return TryIncrenentInt32Int32(surveyId, callId);
            } else if (_inner != null)
            {
                return ((IQuotaClusterService)_inner).TryIncrenent(surveyId, callId);
            }

            return default(bool);
        }

        public delegate bool IncrenentInt32Int32Delegate(int surveyId, int callId);
        public IncrenentInt32Int32Delegate IncrenentInt32Int32;

        bool IQuotaClusterService.Increnent(int surveyId, int callId)
        {


            if (IncrenentInt32Int32 != null)
            {
                return IncrenentInt32Int32(surveyId, callId);
            } else if (_inner != null)
            {
                return ((IQuotaClusterService)_inner).Increnent(surveyId, callId);
            }

            return default(bool);
        }

        public delegate void DecrementInt32Int32Delegate(int surveyId, int cellId);
        public DecrementInt32Int32Delegate DecrementInt32Int32;

        void IQuotaClusterService.Decrement(int surveyId, int cellId)
        {

            if (DecrementInt32Int32 != null)
            {
                DecrementInt32Int32(surveyId, cellId);
            } else if (_inner != null)
            {
                ((IQuotaClusterService)_inner).Decrement(surveyId, cellId);
            }
        }

    }
}