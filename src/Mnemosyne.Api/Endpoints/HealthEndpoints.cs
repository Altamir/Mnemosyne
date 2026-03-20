using Microsoft.EntityFrameworkCore;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.Api.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/health/live", (CancellationToken ct) => Results.Ok(new { status = "alive" }))
            .WithName("HealthLive")
            .WithTags("Health")
            .WithSummary("Liveness probe - returns 200 if service is running");

        app.MapGet("/health/ready", async (MnemosyneDbContext context, CancellationToken ct) =>
        {
            try
            {
                var canConnect = await context.Database.CanConnectAsync(ct);
                if (canConnect)
                {
                    return Results.Ok(new { status = "ready", database = "connected" });
                }
                return Results.Json(new { status = "not ready", database = "disconnected" }, statusCode: 503);
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
