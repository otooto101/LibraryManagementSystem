using System.Threading.RateLimiting;

namespace API.Infrastructure.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddGlobalRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Define a global policy that applies to all endpoints
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var factory = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(factory, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 30,          // Allow 30 request
                        Window = TimeSpan.FromMinutes(1), // per 1 minute
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0             // Do not queue requests just reject them immediately
                    });
                });
            });

            return services;
        }
    }
}
