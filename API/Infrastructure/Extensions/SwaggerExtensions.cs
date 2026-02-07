using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace API.Infrastructure.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Library Management API",
                    Version = "v1",
                    Description = "An API for managing library resources."
                });

                // Add JWT Security Definition
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your valid token in the text input below.\n\nExample: '12345abcdef'"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Enable Example Filters
                options.ExampleFilters();

                // Load XML Comments
                var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
                if (File.Exists(apiXmlPath))
                {
                    options.IncludeXmlComments(apiXmlPath);
                }

                // Versioning Predicate
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;
                    return docName == "v1";
                });
            });

            services.AddSwaggerExamplesFromAssemblyOf<Program>();

            return services;
        }
    }
}