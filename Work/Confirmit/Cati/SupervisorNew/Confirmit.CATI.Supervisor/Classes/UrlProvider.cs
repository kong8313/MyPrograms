using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Supervisor.Classes
{
    public class UrlProvider : IUrlProvider
    {
        private readonly IReviewerService _reviewerService;

        public UrlProvider(IReviewerService reviewerService)
        {
            _reviewerService = reviewerService;
        }

        public string GetReviewerLaunchUrl()
        {
            var urlTemplate = _reviewerService.GetReviewerUrlTemplate();
            
            return UrlHelper.ModifyUrlProtocol(string.Format(urlTemplate, string.Empty));
        }
    }
}
