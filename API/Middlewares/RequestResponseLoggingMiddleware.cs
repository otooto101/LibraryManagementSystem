using Application.Interfaces;

namespace API.Middlewares
{
    public class RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger,
        ITrafficLogService logService,
        IConfiguration config)
    {
        private readonly string[] _ignoredPaths = ["/_framework", "/favicon", "/_vs", "/swagger"];

        public async Task InvokeAsync(HttpContext context)
        {
            bool isEnabled = config.GetValue<bool>("RequestLogging:Enabled", true);

            var path = context.Request.Path.Value?.ToLower() ?? "";

            if (!isEnabled || _ignoredPaths.Any(p => path.Contains(p)))
            {
                await next(context);
                return;
            }

            try
            {
                context.Request.EnableBuffering();
                await logService.SaveLogAsync(
                    context.Request.Body,
                    context.Request.ContentType,
                    context.TraceIdentifier,
                    context.Request.Method,
                    context.Request.Path,
                    "Req");
            }
            catch (Exception ex) 
            { 
                logger.LogWarning("Req Save Failed: {Message}", ex.Message); 
            }

            var originalBodyStream = context.Response.Body;
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                // Continue Pipeline
                await next(context);
            }
            finally
            {
                try
                {
                    await logService.SaveLogAsync(
                        responseBodyStream,
                        context.Response.ContentType,
                        context.TraceIdentifier,
                        context.Request.Method,
                        context.Request.Path,
                        "Res");
                }
                catch (Exception ex) 
                { 
                    logger.LogWarning("Res Save Failed: {Message}", ex.Message); 
                }

                // We re-calculate the folder name here so we can show it in the console
                var safeTraceId = context.TraceIdentifier.Replace(":", "-");
                var folderName = $"{DateTime.Now:HH-mm-ss}_{safeTraceId}";

                logger.LogInformation(
                    "TraceId: {TraceId} | {Method} {Path} | Status: {Status} | Body Files Saved to: {Folder}",
                    context.TraceIdentifier, 
                    context.Request.Method, 
                    context.Request.Path, 
                    context.Response.StatusCode, 
                    folderName);

                // Copy stream back so the client receives the response
                responseBodyStream.Position = 0;
                await responseBodyStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }
    }
}