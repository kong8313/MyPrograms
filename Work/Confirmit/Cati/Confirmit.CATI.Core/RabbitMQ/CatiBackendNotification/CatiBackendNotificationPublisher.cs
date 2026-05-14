using Confirmit.CATI.Core.Misc;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public class CatiBackendNotificationPublisher : ICatiBackendNotificationPublisher
    {
        private const string ExchangeName = "Confirmit.Cati.Backend.Notification";

        private readonly CatiMessageBrokerPublisher _publisher;
        private readonly ICompanyInfo _companyInfo;

        public CatiBackendNotificationPublisher(
            ICompanyInfo companyInfo,
            CatiMessageBrokerPublisher publisher)
        {
            _publisher = publisher;
            _companyInfo = companyInfo;
        }

        public void PublishSurveyLaunched(int surveyId)
        {
            PublishInCurrentCompany(new SurveyLaunchedNotification() { SurveyId = surveyId });
        }

        public void PublishAsyncOperationCancelled(int operationEntityId)
        {
            PublishInCurrentCompany(new AsyncOperationCancelledNotification() { OperationEntityId = operationEntityId });
        }

        private void PublishInCurrentCompany<T>(T notification)
        {
            var jsonContent = JsonConvert.SerializeObject(notification);
            var routingKey = _companyInfo.CompanyId.ToString();
            Publish(jsonContent, notification.GetType().Name, routingKey);
        }

        private void Publish(string jsonContent, string typeName, string routingKey)
        {
            var message = new CatiBackendNotification()
            {
                JsonContent = jsonContent
            };

            _publisher.Publish(ExchangeName, message, topic: routingKey, typeName: typeName);
        }
    }
}