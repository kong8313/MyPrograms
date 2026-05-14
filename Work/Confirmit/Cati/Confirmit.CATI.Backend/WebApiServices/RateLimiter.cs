using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class RateLimiter
    {
        private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> _requestLogByIp;
        private readonly int _maxRequests; 
        private readonly TimeSpan _timeWindow;

        public RateLimiter(int maxRequests, TimeSpan timeWindow)
        {
            _maxRequests = maxRequests;
            _timeWindow = timeWindow;
            _requestLogByIp = new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();
        }

        private string ExtractClientIp(IOwinContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var firstIp = forwardedFor.FirstOrDefault()?.Split(',').Select(ip => ip.Trim()).FirstOrDefault();
                if (!string.IsNullOrEmpty(firstIp))
                {
                    return firstIp;
                }
            }

            return context.Request.RemoteIpAddress ?? 
                   context.Request.Headers.Get("REMOTE_ADDR");
        }

        public bool IsAllowed(IOwinContext context)
        {
            var ip = ExtractClientIp(context);
            if (string.IsNullOrEmpty(ip))
            {
                return true;
            }

            var requests = _requestLogByIp.GetOrAdd(ip, _ => new ConcurrentQueue<DateTime>());
            var now = DateTime.UtcNow;

            while (requests.TryPeek(out var oldestRequest) && now - oldestRequest > _timeWindow)
            {
                requests.TryDequeue(out _);
            }

            if (requests.Count >= _maxRequests)
            {
                return false;
            }

            requests.Enqueue(now);
            return true;
        }
    }
}