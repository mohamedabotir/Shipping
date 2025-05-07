
public class CorrelationMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString(); // Generate a new Correlation ID
            context.Request.Headers[CorrelationIdHeader] = correlationId;
        }

        context.Items[CorrelationIdHeader] = correlationId; // Save for later usage
        context.Response.Headers[CorrelationIdHeader] = correlationId;

        await next(context);
    }
}
