using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification
{
    public class SurveyLaunchedHandler : ICatiBackendNotificationHandler
    {
        private readonly SurveyLaunchedWorker _surveyLaunchedWorker;

        public SurveyLaunchedHandler(SurveyLaunchedWorker surveyLaunchedWorker)
        {
            _surveyLaunchedWorker = surveyLaunchedWorker;
        }

        public string NotificationTypeName => nameof(SurveyLaunchedNotification);

        public Task HandleMessage(CatiBackendNotification message)
        {
            var notification = JsonConvert.DeserializeObject<SurveyLaunchedNotification>(message.JsonContent);
            _surveyLaunchedWorker.Execute(notification);

            return Task.CompletedTask;
        }
    }
}