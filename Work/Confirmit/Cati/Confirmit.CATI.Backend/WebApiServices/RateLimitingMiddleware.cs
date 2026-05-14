using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.SystemSettings;
using Microsoft.Owin;

namespace Confirmit.CATI.Backend.WebApiServices
{
    public class RateLimitingMiddleware : OwinMiddleware
    {
        private readonly List<RateLimiter> _rateLimiters;
        
        public RateLimitingMiddleware(OwinMiddleware next) : base(next)
        {
            _rateLimiters = new List<RateLimiter>
            {
                new RateLimiter(20, TimeSpan.FromSeconds(1)),
                new RateLimiter(1000, TimeSpan.FromMinutes(15)),
                new RateLimiter(10000, TimeSpan.FromHours(12))
            };
        }
        
        public override async Task Invoke(IOwinContext context)
        {
            var requestPath = context.Request.Path.Value ?? string.Empty;
            
            // Skip rate limiting for Swagger and API documentation endpoints
            if (requestPath.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
                requestPath.Equals("/", StringComparison.OrdinalIgnoreCase))
            {
                await Next.Invoke(context);
                return;
            }

            if (ServiceLocator.Resolve<IWebApiSettings>().RateLimiting)
            {
                // Call all limiters to count current request in each one 
                var isAllowed = _rateLimiters.Aggregate(true, (current, rateLimiter) => current && rateLimiter.IsAllowed(context));

                if (!isAllowed)
                {
                    context.Response.StatusCode = 429; // Too Many Requests
                    context.Response.ReasonPhrase = "Rate limit exceeded. Try again later.";
                    return;
                }
            }
            
            await Next.Invoke(context);
        }
    }
}