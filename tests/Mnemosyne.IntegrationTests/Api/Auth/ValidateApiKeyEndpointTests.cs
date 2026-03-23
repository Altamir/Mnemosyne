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

namespace Mnemosyne.IntegrationTests.Api.Auth;

public class ValidateApiKeyEndpointTests : IClassFixture<ValidateApiKeyEndpointTests.MnemosyneWebAppFactory>, IAsyncLifetime
{
    private readonly MnemosyneWebAppFactory _factory;
    private HttpClient _client = null!;
    private const string ValidApiKey = "test-api-key-12345";
    private const string TestEmail = "test@mnemosyne.dev";

    public ValidateApiKeyEndpointTests(MnemosyneWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        // Seed a test user with a known API key
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MnemosyneDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Clear any existing users from previous test runs
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

    [Fact(DisplayName = "Validate API Key - Chave valida retorna 200 com dados do usuario")]
    [Trait("Layer", "Integration - Api")]
    public async Task ValidApiKey_Executed_ReturnsOkWithUserData()
    {
        // Arrange
        var request = new { ApiKey = ValidApiKey };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/validate", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ValidateApiKeyResponse>();
        Assert.NotNull(content);
        Assert.NotEqual(Guid.Empty, content.UserId);
        Assert.Equal(TestEmail, content.Email);
    }

    [Fact(DisplayName = "Validate API Key - Chave invalida retorna 401 Unauthorized")]
    [Trait("Layer", "Integration - Api")]
    public async Task InvalidApiKey_Executed_ReturnsUnauthorized()
    {
        // Arrange
        var request = new { ApiKey = "wrong-api-key" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/validate", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Validate API Key - Chave vazia retorna 400 BadRequest")]
    [Trait("Layer", "Integration - Api")]
    public async Task EmptyApiKey_Executed_ReturnsBadRequest()
    {
        // Arrange
        var request = new { ApiKey = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/validate", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Validate API Key - Chave whitespace retorna 400 BadRequest")]
    [Trait("Layer", "Integration - Api")]
    public async Task WhitespaceApiKey_Executed_ReturnsBadRequest()
    {
        // Arrange
        var request = new { ApiKey = "   " };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/validate", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private record ValidateApiKeyResponse(Guid UserId, string Email);

    /// <summary>
    /// Factory customizada que substitui DbContext por InMemory e registra stubs para servicos ausentes.
    /// </summary>
    public class MnemosyneWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                // Provide a dummy connection string so Program.cs doesn't throw
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:MnemosyneDb"] = "Host=localhost;Database=test;Username=test;Password=test"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Remove all EF Core / DbContext-related registrations to avoid dual-provider conflict
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

                // Register InMemory DbContext with pgvector Embedding column ignored
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

                // Register stub IEmbeddingService (not registered in Program.cs yet — Task 4)
                services.AddSingleton<IEmbeddingService, StubEmbeddingService>();
            });
        }
    }

    /// <summary>
    /// Stub para IEmbeddingService usado nos testes de integracao.
    /// Retorna null (sem embeddings reais).
    /// </summary>
    private class StubEmbeddingService : IEmbeddingService
    {
        public Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Vector?>(null);
        }
    }
}
