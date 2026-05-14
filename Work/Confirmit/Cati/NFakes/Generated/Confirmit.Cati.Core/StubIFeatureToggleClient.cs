using System;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIFeatureToggleClient : IFeatureToggleClient 
    {
        private IFeatureToggleClient _inner;

        public StubIFeatureToggleClient()
        {
            _inner = null;
        }

        public IFeatureToggleClient Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate FeatureToggleAccessResult FeatureToggleAccessStringDelegate(string toggleName);
        public FeatureToggleAccessStringDelegate FeatureToggleAccessString;

        FeatureToggleAccessResult IFeatureToggleClient.FeatureToggleAccess(string toggleName)
        {


            if (FeatureToggleAccessString != null)
            {
                return FeatureToggleAccessString(toggleName);
            } else if (_inner != null)
            {
                return ((IFeatureToggleClient)_inner).FeatureToggleAccess(toggleName);
            }

            return default(FeatureToggleAccessResult);
        }

    }
}