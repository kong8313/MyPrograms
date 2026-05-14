using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Telephony
{
    class DialerService : IDialerService
    {
        private readonly IDialersRepository _dialersRepository;
        private readonly IDialerFeaturesRepository _dialerFeaturesRepository;

        public DialerService(IDialersRepository dialersRepository, IDialerFeaturesRepository dialerFeaturesRepository)
        {
            _dialerFeaturesRepository = dialerFeaturesRepository;
            _dialersRepository = dialersRepository;
        }

        public void DeleteDialerWithFeatures(int dialerId)
        {
            _dialersRepository.Delete(dialerId);
            _dialerFeaturesRepository.DeleteAll(dialerId);
        }
    }

}