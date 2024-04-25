namespace NutriBest.Server.Infrastructure.Middlewares
{
    using Microsoft.Extensions.Caching.Memory;
    using System.Net;

    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IMemoryCache memoryCache;
        private readonly int requestLimit = 100;  // Maximum number of requests
        private readonly TimeSpan timeSpan = TimeSpan.FromMinutes(1);  // Time period for limit

        public RateLimitingMiddleware(RequestDelegate next, IMemoryCache memoryCache)
        {
            this.next = next;
            this.memoryCache = memoryCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress!.ToString(); //should be careful
            var cacheKey = $"RateLimit_{ipAddress}";

            // Check if the IP exists in the cache
            if (!memoryCache.TryGetValue(cacheKey, out int requests))
            {
                // If not in cache, start counting requests
                memoryCache.Set(cacheKey, 1, timeSpan);
            }
            else
            {
                if (requests >= requestLimit)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                    return;
                }

                memoryCache.Set(cacheKey, requests + 1, timeSpan);
            }

            await next(context);
        }
    }
}
