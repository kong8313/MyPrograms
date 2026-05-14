using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confirmit.CATI.Core.ActivityLogging.InterviewerActivityLogging;
using Confirmit.CATI.Core.DAL.Handmade.Entity.Procedure;
using Confirmit.CATI.Core.Services.CallDelivery.Interfaces;
using Confirmit.CATI.Core.Services.Interfaces;

namespace Confirmit.CATI.Core.Services.CallDelivery.Requests
{
    internal class CallRequestRepeatable : ICallRequest
    {
        private readonly IRetryingService _retryingService;

        public ICallRequest Request { get; set; }
        public string Description { get; private set; }

        public CallRequestRepeatable(IRetryingService retryingService, ICallRequest request)
        {
            _retryingService = retryingService;

            Request = request;

            Description = String.Format("CallRequestRepeatable(Request={0})", request.Description);
        }

        public CallRequestResult Execute()
        {
            return _retryingService.Retry(Request.Description, () => Request.Execute());

        }

    }
}
