using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Common.Error;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new
            {
                type = "about:blank",
                title = "An unexpected error occurred",
                status = context.Response.StatusCode,
                traceId = traceId
            };

            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }
}

public static class ErrorHandlingExtensions
{
    public static IApplicationBuilder UseGlobalErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
