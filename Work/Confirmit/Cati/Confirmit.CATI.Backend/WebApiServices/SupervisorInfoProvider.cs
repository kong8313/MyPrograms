using Confirmit.CATI.Core.AuthoringService;
using Confirmit.CATI.Core.WcfServices.Clients;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class SupervisorInfoProvider : ISupervisorInfoProvider
    {
        private CatiSupervisorInfo _info;
        private readonly IAuthorizationKeyProvider _authorizationKeyProvider;
        private readonly IAuthoringService _authoringService;

        public SupervisorInfoProvider(IAuthoringService authoringService,
                                      IAuthorizationKeyProvider authorizationKeyProvider)
        {
            _authorizationKeyProvider = authorizationKeyProvider;
            _authoringService = authoringService;
        }

        public CatiSupervisorInfo GetInfo()
        {
            if (_info == null)
            {
                var key = _authorizationKeyProvider.GetKey();

                _info = _authoringService.GetCatiSupervisorInfo(key);
            }

            return _info;
        }
    }
}
