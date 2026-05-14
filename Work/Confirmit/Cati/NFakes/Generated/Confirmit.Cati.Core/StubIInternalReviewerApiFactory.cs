using System;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.Reviewer.Service.Client;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIInternalReviewerApiFactory : IInternalReviewerApiFactory 
    {
        private IInternalReviewerApiFactory _inner;

        public StubIInternalReviewerApiFactory()
        {
            _inner = null;
        }

        public IInternalReviewerApiFactory Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate IInternalReviewerAPI CreateApiClientStringDelegate(string scopes);
        public CreateApiClientStringDelegate CreateApiClientString;

        IInternalReviewerAPI IInternalReviewerApiFactory.CreateApiClient(string scopes)
        {


            if (CreateApiClientString != null)
            {
                return CreateApiClientString(scopes);
            } else if (_inner != null)
            {
                return ((IInternalReviewerApiFactory)_inner).CreateApiClient(scopes);
            }

            return default(IInternalReviewerAPI);
        }

    }
}