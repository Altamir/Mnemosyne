using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mnemosyne.Api.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        // Liveness probe - basic check if service is running
        app.MapGet("/health/live", async (HealthCheckService healthCheckService, CancellationToken ct) =>
        {
            // Liveness only checks if the application is running, not dependencies
            return Results.Ok(new 
            { 
                status = "healthy",
                timestamp = DateTime.UtcNow
            });
        })
        .WithName("HealthLive")
        .WithTags("Health")
        .WithSummary("Liveness probe - returns 200 if service is running");

        // Readiness probe - checks all registered health checks
        app.MapGet("/health/ready", async (HealthCheckService healthCheckService, CancellationToken ct) =>
        {
            var report = await healthCheckService.CheckHealthAsync(ct);
            
            var result = new
            {
                status = report.Status.ToString().ToLowerInvariant(),
                timestamp = DateTime.UtcNow,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString().ToLowerInvariant(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds
                })
            };

            return report.Status == HealthStatus.Healthy 
                ? Results.Ok(result)
                : Results.Json(result, statusCode: 503);
        })
        .WithName("HealthReady")
        .WithTags("Health")
        .WithSummary("Readiness probe - returns 200 if all dependencies are healthy, 503 otherwise");

        // Detailed health check endpoint
        app.MapGet("/health", async (HealthCheckService healthCheckService, CancellationToken ct) =>
        {
            var report = await healthCheckService.CheckHealthAsync(ct);
            
            var result = new
            {
                status = report.Status.ToString().ToLowerInvariant(),
                timestamp = DateTime.UtcNow,
                totalDuration = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString().ToLowerInvariant(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds,
                    data = e.Value.Data
                })
            };

            return report.Status == HealthStatus.Healthy 
                ? Results.Ok(result)
                : Results.Json(result, statusCode: 503);
        })
        .WithName("HealthDetailed")
        .WithTags("Health")
        .WithSummary("Detailed health check with all registered checks");
    }
}
