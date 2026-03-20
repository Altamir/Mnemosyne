using Microsoft.EntityFrameworkCore;
using Mnemosyne.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace Mnemosyne.IntegrationTests.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    private string? _connectionString;

    public DbContextOptions<MnemosyneDbContext> Options { get; private set; } = null!;

    public PostgresFixture()
    {
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";

        _container = new PostgreSqlBuilder()
            .WithImage("pgvector/pgvector:0.8.2-pg18-trixie")
            .WithUsername(user)
            .WithPassword(password)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();
        Options = new DbContextOptionsBuilder<MnemosyneDbContext>()
            .UseNpgsql(_connectionString)
            .Options;
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
    }
}
