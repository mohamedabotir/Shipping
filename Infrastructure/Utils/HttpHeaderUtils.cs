using Microsoft.AspNetCore.Http;

namespace Infrastructure.Utils;

public static class HttpHeaderUtils{
    public static string GetCorrelationId (this HttpContext client)=> client.Items["X-Correlation-ID"]?.ToString() ?? Guid.NewGuid().ToString();

}