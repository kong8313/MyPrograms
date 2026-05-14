using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.DAL.Generated.Entity.Table;
using Confirmit.CATI.Core.Logger;
using Confirmit.CATI.Core.Repositories.Interfaces;
using Confirmit.CATI.Telephony.DialerCommon.EventNotifications;

namespace Confirmit.SystemTestFramework.Controllers.Dialers
{
    public class DialerController : TestController
    {
        private readonly int _id;
        private readonly IInboundTelephoneNumberRepository _inboundTelephoneNumberRepository;
        private readonly DialerEventsServiceClient _dialerEventsHandlerServiceClient;
        private readonly int _companyId;

        public DialerController(int id)
        {
            _id = id;

            _companyId = int.Parse(Properties.Settings.Default.CompanyId);
            _inboundTelephoneNumberRepository = ServiceLocator.Resolve<IInboundTelephoneNumberRepository>();
            _dialerEventsHandlerServiceClient = new DialerEventsServiceClient(_companyId, new CatiLogger());
        }

        private void AddOrUpdateDdiNumber(string telephonyNumber, int surveyId, int dialerId)
        {
            if (_inboundTelephoneNumberRepository.GetByTelephoneNumbers(new[] { telephonyNumber }).Count > 0)
            {
                _inboundTelephoneNumberRepository.Update(new BvInboundTelephoneNumberEntity
                {
                    TelephoneNumber = telephonyNumber,
                    AudioMessagesJson = "",
                    DialerId = dialerId,
                    SurveyId = surveyId
                });

                return;
            }

            _inboundTelephoneNumberRepository.Insert(new BvInboundTelephoneNumberEntity
            {
                TelephoneNumber = telephonyNumber,
                AudioMessagesJson = "",
                DialerId = dialerId,
                SurveyId = surveyId
            });
        }

        public void SimulateInboundCall(int surveyId, string inboundCallNumber, string callerNumber)
        {
            AddOrUpdateDdiNumber(inboundCallNumber, surveyId, _id);

            _dialerEventsHandlerServiceClient.NotifyInboundCall(_id, _companyId, inboundCallNumber, callerNumber, "");
        }
    }
}