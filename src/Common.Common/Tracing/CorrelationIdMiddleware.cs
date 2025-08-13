using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Common.Common.Tracing;

public sealed class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out StringValues correlationId) || StringValues.IsNullOrEmpty(correlationId))
        {
            correlationId = context.TraceIdentifier;
        }

        context.Items[HeaderName] = correlationId.ToString();
        context.Response.Headers[HeaderName] = correlationId.ToString();
        await _next(context);
    }
}

public static class CorrelationIdExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
