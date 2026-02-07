using Asp.Versioning;

namespace API.Infrastructure.Extensions
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddApiVersioningConfig(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // Default to v1.0 if not specified
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Report versions in headers
                options.ReportApiVersions = true;

                // Read version from the URL Path (e.g. /api/v1/...)
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                // Format version as 'v'major.minor (e.g., v1)
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}
