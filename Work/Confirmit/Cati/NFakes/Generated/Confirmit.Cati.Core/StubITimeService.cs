using System;
using Confirmit.CATI.Core.Services.TimeService;

namespace Confirmit.CATI.Core.Services.TimeService.Fakes
{
    public class StubITimeService : ITimeService 
    {
        private ITimeService _inner;

        public StubITimeService()
        {
            _inner = null;
        }

        public ITimeService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate DateTime GetUtcNowDelegate();
        public GetUtcNowDelegate GetUtcNow;

        DateTime ITimeService.GetUtcNow()
        {


            if (GetUtcNow != null)
            {
                return GetUtcNow();
            } else if (_inner != null)
            {
                return ((ITimeService)_inner).GetUtcNow();
            }

            return default(DateTime);
        }

    }
}