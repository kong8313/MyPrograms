using System.Diagnostics;
using Confirmit.CATI.Core.Repositories.Interfaces;

namespace Confirmit.CATI.Core.Telephony
{
    public class DialerOperationalStateNotificator : IDialerOperationalStateNotificator
    {
        private readonly IDialersRepository _dialersRepository;

        public DialerOperationalStateNotificator(IDialersRepository dialersRepository)
        {
            _dialersRepository = dialersRepository;
        }

        public void SendDialerOperationalStateNotification(int dialerId, bool operational)
        {
            var dialerEntity = _dialersRepository.GetById(dialerId);

            Trace.TraceInformation(
                "SendDialerOperationalStateNotification, dialerId = {0}: OldValue = [{1}], " +
                "NewValue = [{2}], " +
                "Stack = {3}",
                dialerId,
                dialerEntity.DialerOperationalStateNotification,
                operational,
                new StackTrace(true));

            dialerEntity.DialerOperationalStateNotification = operational;

            _dialersRepository.Update(dialerEntity);
        }
    }
}