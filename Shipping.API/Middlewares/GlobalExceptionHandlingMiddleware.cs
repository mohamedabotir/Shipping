using System.Net;
using System.Text.Json;
using Common.Result;
using Infrastructure.Utils;


public class GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.GetCorrelationId();

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred. CorrelationID: {CorrelationId}", correlationId);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var errorResponse = Result.Fail($"An unexpected error occurred. please contact customer support {correlationId}");


            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
