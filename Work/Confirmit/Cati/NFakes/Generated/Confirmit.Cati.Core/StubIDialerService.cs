using System;
using Confirmit.CATI.Core.Telephony;

namespace Confirmit.CATI.Core.Telephony.Fakes
{
    public class StubIDialerService : IDialerService 
    {
        private IDialerService _inner;

        public StubIDialerService()
        {
            _inner = null;
        }

        public IDialerService Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void DeleteDialerWithFeaturesInt32Delegate(int dialerId);
        public DeleteDialerWithFeaturesInt32Delegate DeleteDialerWithFeaturesInt32;

        void IDialerService.DeleteDialerWithFeatures(int dialerId)
        {

            if (DeleteDialerWithFeaturesInt32 != null)
            {
                DeleteDialerWithFeaturesInt32(dialerId);
            } else if (_inner != null)
            {
                ((IDialerService)_inner).DeleteDialerWithFeatures(dialerId);
            }
        }

    }
}