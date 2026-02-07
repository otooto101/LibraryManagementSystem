using API.Infrastructure.Extensions;
using API.Infrastructure.Mappings;
using API.Middlewares;
using Infrastructure.Extensions;
using Application;
using Infrastructure;
using Persistence;
using Serilog;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Text.Json.Serialization;

// Calculate Today's Path
var today = DateTime.Now.ToString("yyyy-MM-dd");
var logPath = Path.Combine("Logs", today, $"library-system-.txt");

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting the Library Management System...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, shared: true));

    builder.Services.AddApplication();
    builder.Services.AddPersistence(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddBackgroundJobs();
    builder.Services.AddGlobalRateLimiting();

    builder.Services.AddApiVersioningConfig();

    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddSwaggerServices();
    builder.Services.AddHttpContextAccessor();

    var app = builder.Build();

    MappingConfigApi.ConfigureModels(app.Services);

    app.UseMiddleware<RequestResponseLoggingMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();

    if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API v1");
            c.DisplayRequestDuration();
        });
    }

    app.UseHttpsRedirection();

    app.UseRateLimiter();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Seeding Database...");
    await app.SeedDatabaseAsync();

    Log.Information("Application is running!");
    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException) // added to avoid migration problems
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}