using System;
using System.Linq;
using Confirmit.CATI.Common;
using Confirmit.CATI.Core.Misc;
using Confirmit.Configuration.Bootstrap;
using ConfirmitDialerInterface;
using Prometheus;

namespace Confirmit.CATI.Core
{
    public static class CustomMetrics
    {
        private static readonly double[] Buckets = new[] { .005, .01, .025, .05, .1, .25, .5, 1, 2.5, 5, 10, 25, 50 };
        
        private static readonly Histogram WcfRequestTime = Metrics.CreateHistogram(
            "cati_backend_wcf_request_time_seconds",
            "Histogram of the time spent in WCF methods in CATI backend. In seconds.",
            new HistogramConfiguration
            {
                Buckets = Buckets,
                LabelNames = GetLabelNames().Union(new[] { "service", "method" }).ToArray()
            });
        
        private static readonly Histogram ActivityEventTime = Metrics.CreateHistogram(
            "cati_activity_event_time_seconds",
            "Histogram of the time spent in activity events. In seconds.",
            new HistogramConfiguration
            {
                Buckets = Buckets,
                LabelNames = GetLabelNames().Union(new[] { "type", "name" }).ToArray()
            });
        
        private static readonly Histogram WebApiRequestTime = Metrics.CreateHistogram(
            "cati_backend_web_api_request_time_seconds",
            "Histogram of the time spent in old CATI REST API requests. In seconds.",
            new HistogramConfiguration
            {
                Buckets = Buckets,
                LabelNames = GetLabelNames().Union(new[] { "status" }).ToArray()
            });
        
        private static readonly Counter DialerCallsRequested = Metrics.CreateCounter(
            "cati_dialer_calls_requested_count",
            "Total number of calls requested by the predictive dialer",
            new CounterConfiguration()
            {
                LabelNames = GetLabelNames().Union(new[] { "requestType"}).ToArray(),
            });
        
        private static readonly Counter DialerCallsSent = Metrics.CreateCounter(
            "cati_dialer_calls_sent_count",
            "Total number of calls sent to the predictive dialer",
            new CounterConfiguration()
            {
                LabelNames = GetLabelNames().Union(new[] { "requestType"}).ToArray(),
            });
        
        private static readonly Counter DialerCallOutcome = Metrics.CreateCounter(
            "cati_dialer_call_outcome_count",
            "Total number of call outcomes recieved from the dialer",
            new CounterConfiguration()
            {
                LabelNames = GetLabelNames().Union(new[] { "dialerId", "outcome", "dialType"}).ToArray(),
            });
        
        private static readonly Counter CacheHitCount = Metrics.CreateCounter(
            "cati_backend_cache_hit_count",
            "Total count of cache hits in CATI backend",
            new CounterConfiguration()
            {
                LabelNames = GetLabelNames().ToArray(),
            });
        
        private static readonly Counter CacheMissCount = Metrics.CreateCounter(
            "cati_backend_cache_miss_count",
            "Total count of cache misses in CATI backend",
            new CounterConfiguration()
            {
                LabelNames = GetLabelNames().ToArray(),
            });
        
        private static readonly Counter CacheRereadCount = Metrics.CreateCounter(
            "cati_backend_cache_reread_count",
            "Total number of cache rereads in CATI backend",
            new CounterConfiguration()
            {
                LabelNames = GetLabelNames().ToArray(),
            });


        public static ITimer OnWcfRequest(string service, string method)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return null;
                
            return WcfRequestTime.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName,
                service,
                method
            ).NewTimer();
        }
        
        public static void OnActivityEvent(string type, string name, TimeSpan duration)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;
            
            ActivityEventTime.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName,
                type,
                name
            ).Observe(duration.TotalSeconds);
        }
        
        public static void OnWebApiRequest(string status, TimeSpan duration)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;
            
            WebApiRequestTime.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName,
                status
            ).Observe(duration.TotalSeconds);
        }
        
        public static void OnSendCallsToDialer(CallsSelectionAlgorithm requestType, int requestedCalls, int sentCalls)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;
            
            DialerCallsRequested.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName,
                requestType.ToString()).Inc(requestedCalls);
                
            DialerCallsSent.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName,
                requestType.ToString()).Inc(sentCalls);
        } 
        
        public static void OnCallOutcome(int dialerId, CallOutcome callOutcome, DialType? dialType)
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;

            DialerCallOutcome.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName,
                dialerId.ToString(),
                callOutcome.ToString(),
                dialType?.ToString() ?? "Unknown"
            ).Inc();
        }
        
        public static void OnCacheHit()
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;
            
            CacheHitCount.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName
            ).Inc();
        }
        
        public static void OnCacheMiss()
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;
            
            CacheMissCount.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName
            ).Inc();
        }
        
        public static void OnCacheReread()
        {
            if (!BootstrapConfig.IsContainerEnvironment)
                return;
            
            CacheRereadCount.WithLabels(
                BackendInstance.Current.CompanyId.ToString(),
                BackendInstance.Current.CompanyName
            ).Inc();
        }


        private static string[] GetLabelNames() => new[]
        {
            "companyId",
            "company",
        };
    }
}
