namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public interface ICatiBackendNotificationPublisher
    {
        void PublishSurveyLaunched(int surveyId);
        void PublishAsyncOperationCancelled(int operationEntityId);
    }
}