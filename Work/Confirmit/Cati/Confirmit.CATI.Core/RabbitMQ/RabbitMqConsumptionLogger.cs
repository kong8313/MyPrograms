using System;
using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.MessageBroker.Consume.Sdk;
using Microsoft.Extensions.Logging;

namespace Confirmit.CATI.Core.RabbitMQ
{
    public class RabbitMqConsumptionLogger<T> : ILogger<RabbitMQConsumption<T>> where T : class
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (exception != null)
                Trace.TraceError($"RabbitMqConsumption error. Exception details: {exception}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new DisposablePlug();
        }

        private class DisposablePlug : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}