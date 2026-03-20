using Microsoft.EntityFrameworkCore;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.Api.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/health/live", () => Results.Ok(new { status = "alive" }))
            .WithName("HealthLive")
            .WithTags("Health")
            .WithSummary("Liveness probe - returns 200 if service is running");

        app.MapGet("/health/ready", async (MnemosyneDbContext context) =>
        {
            try
            {
                var canConnect = await context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Results.Ok(new { status = "ready", database = "connected" });
                }
                return Results.Json(new { status = "not ready", database = "disconnected" }, statusCode: 503);
            }
            catch (Exception ex) when (ex is NotSupportedException)
            {
                return Results.Ok(new { status = "ready", database = "inmemory" });
            }
            catch (Exception)
            {
                return Results.Json(new { status = "not ready", database = "error" }, statusCode: 503);
            }
        })
        .WithName("HealthReady")
        .WithTags("Health")
        .WithSummary("Readiness probe - returns 200 if database is accessible, 503 otherwise");
    }
}
