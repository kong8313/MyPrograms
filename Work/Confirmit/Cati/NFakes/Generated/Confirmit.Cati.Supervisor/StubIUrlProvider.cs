using System;
using Confirmit.CATI.Supervisor.Classes;

namespace Confirmit.CATI.Supervisor.Classes.Fakes
{
    public class StubIUrlProvider : IUrlProvider 
    {
        private IUrlProvider _inner;

        public StubIUrlProvider()
        {
            _inner = null;
        }

        public IUrlProvider Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate string GetReviewerLaunchUrlDelegate();
        public GetReviewerLaunchUrlDelegate GetReviewerLaunchUrl;

        string IUrlProvider.GetReviewerLaunchUrl()
        {


            if (GetReviewerLaunchUrl != null)
            {
                return GetReviewerLaunchUrl();
            } else if (_inner != null)
            {
                return ((IUrlProvider)_inner).GetReviewerLaunchUrl();
            }

            return default(string);
        }

    }
}