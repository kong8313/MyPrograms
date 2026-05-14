using Confirmit.CATI.Common;

namespace Confirmit.CATI.Core.Services.CallDelivery.Interfaces
{
    internal interface ICallRequestFactory
    {
        ICallRequest Create(int personId, int surveyId, int interviewId);
    }
}
