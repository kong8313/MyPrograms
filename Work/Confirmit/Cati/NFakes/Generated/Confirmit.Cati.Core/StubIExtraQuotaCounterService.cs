using System;
using Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation;

namespace Confirmit.CATI.Core.Services.ExtraQuotaCounterServiceImplementation.Fakes
{
    public class StubIExtraQuotaCounterService : IExtraQuotaCounterService 
    {
        private IExtraQuotaCounterService _inner;

        public StubIExtraQuotaCounterService()
        {
            _inner = null;
        }

        public IExtraQuotaCounterService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IExtraQuotaCounterCalculator CreateIExtraQuotaCounterParametersDelegate(IExtraQuotaCounterParameters parameters);
        public CreateIExtraQuotaCounterParametersDelegate CreateIExtraQuotaCounterParameters;

        IExtraQuotaCounterCalculator IExtraQuotaCounterService.Create(IExtraQuotaCounterParameters parameters)
        {


            if (CreateIExtraQuotaCounterParameters != null)
            {
                return CreateIExtraQuotaCounterParameters(parameters);
            } else if (_inner != null)
            {
                return ((IExtraQuotaCounterService)_inner).Create(parameters);
            }

            return default(IExtraQuotaCounterCalculator);
        }

    }
}