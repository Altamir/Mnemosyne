using Mnemosyne.Application.Features.Auth.ValidateApiKey;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "X-Api-Key";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ValidateApiKeyHandler handler)
    {
        // Skip authentication for auth endpoints, health checks, and documentation
        if (context.Request.Path.StartsWithSegments("/api/v1/auth") ||
            context.Request.Path.StartsWithSegments("/health") ||
            context.Request.Path.StartsWithSegments("/scalar") ||
            context.Request.Path.StartsWithSegments("/openapi"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API Key not provided" });
            return;
        }

        try
        {
            var query = new ValidateApiKeyQuery(extractedApiKey.ToString());
            var user = await handler.Handle(query, CancellationToken.None);

            if (user is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key" });
                return;
            }

            context.Items["UserId"] = user.Id;
            await _next(context);
        }
        catch (ArgumentException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API Key format" });
        }
    }
}

public static class ApiKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiKeyMiddleware>();
    }
}