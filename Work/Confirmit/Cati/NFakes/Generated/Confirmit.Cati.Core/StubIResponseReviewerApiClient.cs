using System;
using Confirmit.CATI.Core.Services;
using Confirmit.CATI.Core.Services.Interfaces;
using System.Threading.Tasks;

namespace Confirmit.CATI.Core.Services.Interfaces.Fakes
{
    public class StubIResponseReviewerApiClient : IResponseReviewerApiClient 
    {
        private IResponseReviewerApiClient _inner;

        public StubIResponseReviewerApiClient()
        {
            _inner = null;
        }

        public IResponseReviewerApiClient Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate Task<SessionModel> AddSessionSessionModelDelegate(SessionModel sessionModel);
        public AddSessionSessionModelDelegate AddSessionSessionModel;

        Task<SessionModel> IResponseReviewerApiClient.AddSession(SessionModel sessionModel)
        {


            if (AddSessionSessionModel != null)
            {
                return AddSessionSessionModel(sessionModel);
            } else if (_inner != null)
            {
                return ((IResponseReviewerApiClient)_inner).AddSession(sessionModel);
            }

            return default(Task<SessionModel>);
        }

    }
}