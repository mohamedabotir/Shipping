using System.Diagnostics;
using Infrastructure.Utils;


public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.GetCorrelationId(); 
        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;
        var responseBody = string.Empty;
        try
        {
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;
            
            await next(context); 
            
            memoryStream.Seek(0, SeekOrigin.Begin);
            responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
        }
        finally
        {
            stopwatch.Stop();
            var processingTime = stopwatch.ElapsedMilliseconds;
            var statusCode = context.Response.StatusCode;

            logger.LogInformation("Request {Method} {Path} responded {StatusCode} in {ProcessingTimeInMS}ms - CorrelationID: {CorrelationId},Response: {Response}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                processingTime,
                correlationId,responseBody);
        }
    }
}