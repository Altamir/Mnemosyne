using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;
using Pgvector;

namespace Mnemosyne.IntegrationTests.Api.OpenApi;

[Trait("Layer", "Integration - OpenAPI")]
public class OpenApiDocumentTests : IClassFixture<OpenApiDocumentTests.OpenApiWebAppFactory>, IAsyncLifetime
{
    private readonly OpenApiWebAppFactory _factory;
    private HttpClient _client = null!;

    public OpenApiDocumentTests(OpenApiWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Add("X-Api-Key", OpenApiWebAppFactory.ApiKey);
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MnemosyneDbContext>();
        await context.Database.EnsureCreatedAsync();
        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();
        var user = UserEntity.Create(OpenApiWebAppFactory.ApiKey, "openapi-test@mnemosyne.dev");
        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Documento OpenAPI deve declarar security scheme ApiKey")]
    [Trait("Layer", "Integration - OpenAPI")]
    public async Task SecurityScheme_Declared_ContainsApiKey()
    {
        // Act
        var response = await _client.GetAsync("/openapi/v1.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var components = doc.RootElement.GetProperty("components");
        var securitySchemes = components.GetProperty("securitySchemes");
        Assert.True(securitySchemes.TryGetProperty("ApiKey", out var scheme));
        Assert.Equal("apiKey", scheme.GetProperty("type").GetString());
        Assert.Equal("X-Api-Key", scheme.GetProperty("name").GetString());
        Assert.Equal("header", scheme.GetProperty("in").GetString());
    }

    [Fact(DisplayName = "Documento OpenAPI deve ter security requirement global")]
    [Trait("Layer", "Integration - OpenAPI")]
    public async Task SecurityRequirement_Global_IsDeclared()
    {
        // Act
        var response = await _client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var security = doc.RootElement.GetProperty("security");
        Assert.True(security.EnumerateArray().Any());
    }

    [Fact(DisplayName = "Documento OpenAPI deve retornar 401 sem API key")]
    [Trait("Layer", "Integration - OpenAPI")]
    public async Task OpenApiDocument_WithoutApiKey_Returns401()
    {
        using var unauthenticatedClient = _factory.CreateClient();

        // Act
        var response = await unauthenticatedClient.GetAsync("/openapi/v1.json");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Scalar UI deve retornar 401 sem API key")]
    [Trait("Layer", "Integration - OpenAPI")]
    public async Task ScalarUi_WithoutApiKey_Returns401()
    {
        using var unauthenticatedClient = _factory.CreateClient();

        // Act
        var response = await unauthenticatedClient.GetAsync("/scalar/v1");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public class OpenApiWebAppFactory : WebApplicationFactory<Program>
    {
        public const string ApiKey = "test-api-key-openapi";
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

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
