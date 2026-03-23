using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;
using Pgvector;

namespace Mnemosyne.IntegrationTests.Api.Health;

public class DetailedHealthChecksTests : IClassFixture<DetailedHealthChecksTests.HealthWebAppFactory>, IAsyncLifetime
{
    private readonly HealthWebAppFactory _factory;
    private HttpClient _client = null!;
    private const string ValidApiKey = "test-api-key-health";
    private const string TestEmail = "health-test@mnemosyne.dev";

    public DetailedHealthChecksTests(HealthWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MnemosyneDbContext>();
        await context.Database.EnsureCreatedAsync();
        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        var user = UserEntity.Create(ValidApiKey, TestEmail);
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Health Live - Retorna 200 OK")]
    [Trait("Layer", "Integration - Health")]
    public async Task HealthLive_Executed_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/health/live");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "Health Ready - Retorna status code valido")]
    [Trait("Layer", "Integration - Health")]
    public async Task HealthReady_Executed_ReturnsValidStatus()
    {
        // Act
        var response = await _client.GetAsync("/health/ready");

        // Assert - Should return either 200 (healthy) or 503 (unhealthy)
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.ServiceUnavailable,
            $"Health ready should return 200 or 503, but returned {(int)response.StatusCode}");
    }

    [Fact(DisplayName = "Health Detailed - Retorna status code valido")]
    [Trait("Layer", "Integration - Health")]
    public async Task HealthDetailed_Executed_ReturnsValidStatus()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert - Should return either 200 (healthy) or 503 (unhealthy)
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.ServiceUnavailable,
            $"Health detailed should return 200 or 503, but returned {(int)response.StatusCode}");
    }

    [Fact(DisplayName = "Health Live - Retorna JSON com status")]
    [Trait("Layer", "Integration - Health")]
    public async Task HealthLive_Executed_ReturnsJsonWithStatus()
    {
        // Act
        var response = await _client.GetAsync("/health/live");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("status", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Health Ready - Retorna JSON com checks")]
    [Trait("Layer", "Integration - Health")]
    public async Task HealthReady_Executed_ReturnsJsonWithChecks()
    {
        // Act
        var response = await _client.GetAsync("/health/ready");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.ServiceUnavailable,
            $"Health ready should return 200 or 503, but returned {(int)response.StatusCode}");
        Assert.Contains("checks", content, StringComparison.OrdinalIgnoreCase);
    }

    public class HealthWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:MnemosyneDb"] = "Host=localhost;Database=test;Username=test;Password=test",
                    ["OpenAI:ApiKey"] = "sk-dummy-key-for-tests",
                    ["OpenAI:EmbeddingModel"] = "text-embedding-3-small"
                });
            });

            builder.ConfigureServices(services =>
            {
                var efCoreDescriptors = services
                    .Where(d =>
                        d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                        d.ServiceType.FullName?.Contains("EntityFramework") == true ||
                        d.ServiceType == typeof(DbContextOptions<MnemosyneDbContext>) ||
                        d.ServiceType == typeof(DbContextOptions) ||
                        d.ServiceType == typeof(MnemosyneDbContext) ||
                        d.ImplementationType?.FullName?.Contains("Npgsql") == true)
                    .ToList();

                foreach (var descriptor in efCoreDescriptors)
                    services.Remove(descriptor);

                var dbContextOptions = new DbContextOptionsBuilder<MnemosyneDbContext>()
                    .UseInMemoryDatabase(_dbName)
                    .Options;

                services.AddSingleton(dbContextOptions);
                services.AddScoped(sp => new MnemosyneDbContext(
                    sp.GetRequiredService<DbContextOptions<MnemosyneDbContext>>(),
                    modelBuilder =>
                    {
                        modelBuilder.Entity<MemoryEntity>(entity =>
                        {
                            entity.Ignore(e => e.Embedding);
                        });
                    }));

                services.AddSingleton<IEmbeddingService, StubEmbeddingService>();
            });
        }
    }

    private class StubEmbeddingService : IEmbeddingService
    {
        public Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Vector?>(null);
        }
    }
}
