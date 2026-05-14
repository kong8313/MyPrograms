using System;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.Services.Fakes
{
    public class StubIQuotaBalancingParametersValidator : IQuotaBalancingParametersValidator 
    {
        private IQuotaBalancingParametersValidator _inner;

        public StubIQuotaBalancingParametersValidator()
        {
            _inner = null;
        }

        public IQuotaBalancingParametersValidator Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void CheckQuotaBalancingParametersInt32Int32Int32ArrayOfStringInt32Delegate(int surveyId, int quotaId, int promotionPriority, string[] filterFields, int promotionThreshold);
        public CheckQuotaBalancingParametersInt32Int32Int32ArrayOfStringInt32Delegate CheckQuotaBalancingParametersInt32Int32Int32ArrayOfStringInt32;

        void IQuotaBalancingParametersValidator.CheckQuotaBalancingParameters(int surveyId, int quotaId, int promotionPriority, string[] filterFields, int promotionThreshold)
        {

            if (CheckQuotaBalancingParametersInt32Int32Int32ArrayOfStringInt32 != null)
            {
                CheckQuotaBalancingParametersInt32Int32Int32ArrayOfStringInt32(surveyId, quotaId, promotionPriority, filterFields, promotionThreshold);
            } else if (_inner != null)
            {
                ((IQuotaBalancingParametersValidator)_inner).CheckQuotaBalancingParameters(surveyId, quotaId, promotionPriority, filterFields, promotionThreshold);
            }
        }

    }
}