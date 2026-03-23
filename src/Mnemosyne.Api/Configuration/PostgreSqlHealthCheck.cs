using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.Api.Configuration;

public class PostgreSqlHealthCheck : IHealthCheck
{
    private readonly MnemosyneDbContext _context;

    public PostgreSqlHealthCheck(MnemosyneDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            
            if (canConnect)
            {
                return HealthCheckResult.Healthy("PostgreSQL database is accessible");
            }
            
            return HealthCheckResult.Unhealthy("Cannot connect to PostgreSQL database");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                $"PostgreSQL health check failed: {ex.Message}",
                ex);
        }
    }
}
