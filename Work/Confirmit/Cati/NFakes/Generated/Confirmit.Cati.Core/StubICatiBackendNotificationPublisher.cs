using System;
using Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification;

namespace Confirmit.CATI.Core.RabbitMQ.CatiBackendNotification.Fakes
{
    public class StubICatiBackendNotificationPublisher : ICatiBackendNotificationPublisher 
    {
        private ICatiBackendNotificationPublisher _inner;

        public StubICatiBackendNotificationPublisher()
        {
            _inner = null;
        }

        public ICatiBackendNotificationPublisher Inner
        {
            set {_inner = value;} get {return _inner;}
        }

        public delegate void PublishSurveyLaunchedInt32Delegate(int surveyId);
        public PublishSurveyLaunchedInt32Delegate PublishSurveyLaunchedInt32;

        void ICatiBackendNotificationPublisher.PublishSurveyLaunched(int surveyId)
        {

            if (PublishSurveyLaunchedInt32 != null)
            {
                PublishSurveyLaunchedInt32(surveyId);
            } else if (_inner != null)
            {
                ((ICatiBackendNotificationPublisher)_inner).PublishSurveyLaunched(surveyId);
            }
        }

        public delegate void PublishAsyncOperationCancelledInt32Delegate(int operationEntityId);
        public PublishAsyncOperationCancelledInt32Delegate PublishAsyncOperationCancelledInt32;

        void ICatiBackendNotificationPublisher.PublishAsyncOperationCancelled(int operationEntityId)
        {

            if (PublishAsyncOperationCancelledInt32 != null)
            {
                PublishAsyncOperationCancelledInt32(operationEntityId);
            } else if (_inner != null)
            {
                ((ICatiBackendNotificationPublisher)_inner).PublishAsyncOperationCancelled(operationEntityId);
            }
        }

    }
}