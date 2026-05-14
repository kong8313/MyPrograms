using System;
using Confirmit.CATI.Supervisor.Core.Quotas;
using System.Collections.Generic;

namespace Confirmit.CATI.Supervisor.Core.Quotas.Fakes
{
    public class StubIQuotaNameProvider : IQuotaNameProvider 
    {
        private IQuotaNameProvider _inner;

        public StubIQuotaNameProvider()
        {
            _inner = null;
        }

        public IQuotaNameProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IEnumerable<string> GetQuotaNamesInt32Delegate(int surveySid);
        public GetQuotaNamesInt32Delegate GetQuotaNamesInt32;

        IEnumerable<string> IQuotaNameProvider.GetQuotaNames(int surveySid)
        {


            if (GetQuotaNamesInt32 != null)
            {
                return GetQuotaNamesInt32(surveySid);
            } else if (_inner != null)
            {
                return ((IQuotaNameProvider)_inner).GetQuotaNames(surveySid);
            }

            return default(IEnumerable<string>);
        }

    }
}