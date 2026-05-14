using System.Linq;

using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Core.SystemSettings;
using Confirmit.CATI.Core.Telephony;
using Confirmit.CATI.Telephony;

namespace BvCallHandlerLibrary.Tools
{
    public class MnTciTools : IMnTciTools
    {
        private readonly IDialerSettings _dialerSettings;
        private readonly IDialerType _dialerType;
        private readonly IDialersRepository _dialersRepository;

        public MnTciTools(
            IDialerSettings dialerSettings,
            IDialerType dialerType,
            IDialersRepository dialersRepository)
        {
            _dialerSettings = dialerSettings;
            _dialerType = dialerType;
            _dialersRepository = dialersRepository;
        }

        public IDialerRecordingAPI CreateDialerRecording(int dialerId)
        {
            var dialerEntity = _dialersRepository.GetById(dialerId);

            if (string.IsNullOrEmpty(dialerEntity.ConnectionParameters))
            {
                throw new DialerIsNotConfiguredException(
                    string.Format(
                    "Attempt to initialize dialer but connectionParametersXml is null or empty, dialerId = {0}, dialerName = {1}",
                    dialerId,
                    dialerEntity.Name));
            }

            var dialerRecording = _dialerType.CreateInstance<IDialerRecordingAPI>();

            dialerRecording.Initialize(dialerEntity.ConnectionParameters, dialerEntity.ConfigurationParameters);

            return dialerRecording;
        }

        public bool IsDialerConfigured()
        {
            if (!DoesCompanyUseTelephony())
            {
                return false;
            }

            return _dialersRepository.GetAll().Any(); // There is at least one dialer
        }

        public bool DoesCompanyUseTelephony()
        {
            return _dialerSettings.Dialer != DiallerType.NoDialler;
        }
    }
}
