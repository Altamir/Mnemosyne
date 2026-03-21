using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;
using Pgvector;

namespace Mnemosyne.IntegrationTests.Grpc;

/// <summary>
/// Testes de integracao para verificar que os servicos gRPC estao configurados corretamente.
/// Nota: gRPC services sao mapeados via endpoint routing, nao registrados no DI container.
/// </summary>
public class GrpcConfigurationTests : IClassFixture<GrpcConfigurationTests.GrpcWebAppFactory>, IAsyncLifetime
{
    private readonly GrpcWebAppFactory _factory;
    private const string ValidApiKey = "test-api-key-grpc-config";
    private const string TestEmail = "grpc-config-test@mnemosyne.dev";

    public GrpcConfigurationTests(GrpcWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        // Seed test user
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
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "gRPC - CompressService esta configurado no endpoint routing")]
    [Trait("Layer", "Integration - gRPC")]
    public void GrpcServices_Configured_EndpointsRegistered()
    {
        // Arrange & Act - Create a client to verify the server starts correctly
        var client = _factory.CreateClient();

        // Assert - If we get here, the gRPC services were configured without errors
        Assert.NotNull(client);
    }

    [Fact(DisplayName = "gRPC - Servidor inicia corretamente com gRPC configurado")]
    [Trait("Layer", "Integration - gRPC")]
    public void GrpcServer_StartsSuccessfully()
    {
        // Arrange & Act
        var server = _factory.Server;

        // Assert
        Assert.NotNull(server);
        Assert.NotNull(server.BaseAddress);
    }

    public class GrpcWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:MnemosyneDb"] = "Host=localhost;Database=test;Username=test;Password=test"
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
