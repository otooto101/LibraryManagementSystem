using API.Middlewares.Configuration;
using API.Models;
using System.Net;

namespace API.Middlewares
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var status = ExceptionConfiguration.GetStatusCode(exception);

            if (status == HttpStatusCode.InternalServerError)
            {
                logger.LogError(exception, "An unexpected error occurred: {Message}", exception.Message);
            }
            else
            {

                logger.LogWarning("({StatusCode}) {ErrorType}: {Message}",
                    (int)status,
                    exception.GetType().Name,
                    exception.Message);
            }

            var message = status == HttpStatusCode.InternalServerError
                ? "An unexpected internal error occurred."
                : exception.Message;

            var response = new ErrorResponse(
                (int)status,
                message,
                exception.GetType().Name,
                context.TraceIdentifier
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}