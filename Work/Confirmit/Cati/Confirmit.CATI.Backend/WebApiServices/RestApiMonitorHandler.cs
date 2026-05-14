using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Confirmit.CATI.Backend.WebApiServices.Filters;
using Confirmit.CATI.Backend.WebApiServices.Logging;
using Confirmit.CATI.Core.Misc;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class RestApiMonitorHandler : DelegatingHandler, IRestApiMonitorHandler
    {
        private readonly IRestApiMonitorLogger _restApiMonitorLogger;
        private readonly IAsyncManager _asyncManager;

        public RestApiMonitorHandler(
            IRestApiMonitorLogger restApiMonitorLogger,
            IAsyncManager asyncManager)
        {
            _restApiMonitorLogger = restApiMonitorLogger;
            _asyncManager = asyncManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var info = request.Resolve<IRestApiMonitorInfoKeeper>()?.GetInfo();

            if (info != null)
            {
                if (info.Exception != null)
                {
                    info.StatusCode = response.StatusCode;
                    info.ContentType = response.Content != null ? response.Content.Headers.ContentType.MediaType : null;
                }

                _asyncManager.QueueWorkItem(() => _restApiMonitorLogger.Log(info));
            }

            return response;
        }
    }
}
