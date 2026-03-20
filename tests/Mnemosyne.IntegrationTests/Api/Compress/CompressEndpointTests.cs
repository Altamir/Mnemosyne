using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mnemosyne.Api.Endpoints;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;
using Pgvector;

namespace Mnemosyne.IntegrationTests.Api.Compress;

public class CompressEndpointTests : IClassFixture<CompressEndpointTests.CompressWebAppFactory>, IAsyncLifetime
{
    private readonly CompressWebAppFactory _factory;
    private HttpClient _client = null!;
    private const string ValidApiKey = "test-api-key-compress";
    private const string TestEmail = "compress-test@mnemosyne.dev";

    public CompressEndpointTests(CompressWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MnemosyneDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Clear existing data from previous test runs
        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        // Seed a test user for API key authentication
        var user = UserEntity.Create(ValidApiKey, TestEmail);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Set default API key header for all requests
        _client.DefaultRequestHeaders.Add("X-Api-Key", ValidApiKey);
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Compress - Codigo valido retorna 200 OK com resultado comprimido")]
    [Trait("Layer", "Integration - Api")]
    public async Task ValidCode_Compress_ReturnsCompressedResult()
    {
        // Arrange
        var code = @"
            using System;
            namespace Test {
                public class Example {
                    public void Method() {
                        Console.WriteLine(""Hello World"");
                    }
                }
            }";
        var request = new CompressContextRequest(code, CompressionStrategy.CodeStructure, 0.7);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/compress", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<CompressResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.CompressedContent);
        Assert.True(result.OriginalLength > 0);
        Assert.True(result.CompressedLength >= 0);
        Assert.Equal("CodeStructure", result.StrategyUsed);
    }

    [Fact(DisplayName = "Compress - Conteudo vazio retorna 400 BadRequest")]
    [Trait("Layer", "Integration - Api")]
    public async Task EmptyContent_Compress_ReturnsBadRequest()
    {
        // Arrange
        var request = new CompressContextRequest("", CompressionStrategy.CodeStructure, 0.7);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/compress", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Compress - Conteudo whitespace retorna 400 BadRequest")]
    [Trait("Layer", "Integration - Api")]
    public async Task WhitespaceContent_Compress_ReturnsBadRequest()
    {
        // Arrange
        var request = new CompressContextRequest("   ", CompressionStrategy.CodeStructure, 0.7);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/compress", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory(DisplayName = "Compress - TargetRatio invalido retorna 400 BadRequest")]
    [Trait("Layer", "Integration - Api")]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(-0.1)]
    [InlineData(1.5)]
    public async Task InvalidTargetRatio_Compress_ReturnsBadRequest(double targetRatio)
    {
        // Arrange
        var request = new CompressContextRequest("valid content", CompressionStrategy.CodeStructure, targetRatio);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/compress", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Compress - Sem API Key retorna 401 Unauthorized")]
    [Trait("Layer", "Integration - Api")]
    public async Task NoApiKey_Compress_ReturnsUnauthorized()
    {
        // Arrange
        var clientWithoutAuth = _factory.CreateClient();
        var request = new CompressContextRequest("valid content", CompressionStrategy.CodeStructure, 0.7);

        // Act
        var response = await clientWithoutAuth.PostAsJsonAsync("/api/v1/compress", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Compress - API Key invalida retorna 401 Unauthorized")]
    [Trait("Layer", "Integration - Api")]
    public async Task InvalidApiKey_Compress_ReturnsUnauthorized()
    {
        // Arrange
        var clientWithInvalidAuth = _factory.CreateClient();
        clientWithInvalidAuth.DefaultRequestHeaders.Add("X-Api-Key", "invalid-api-key");
        var request = new CompressContextRequest("valid content", CompressionStrategy.CodeStructure, 0.7);

        // Act
        var response = await clientWithInvalidAuth.PostAsJsonAsync("/api/v1/compress", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Compress - Codigo C# complexo comprime corretamente")]
    [Trait("Layer", "Integration - Api")]
    public async Task ComplexCode_Compress_ReturnsCompressedResult()
    {
        // Arrange
        var complexCode = @"
            using System;
            using System.Collections.Generic;
            using System.Linq;

            namespace MyApp.Services
            {
                public class UserService : IUserService
                {
                    private readonly IUserRepository _repository;
                    private readonly ILogger<UserService> _logger;

                    public UserService(IUserRepository repository, ILogger<UserService> logger)
                    {
                        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
                        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
                    }

                    public async Task<User> GetUserByIdAsync(Guid id)
                    {
                        if (id == Guid.Empty)
                        {
                            throw new ArgumentException(""Id cannot be empty"", nameof(id));
                        }

                        _logger.LogInformation(""Getting user by id: {UserId}"", id);
                        
                        var user = await _repository.GetByIdAsync(id);
                        
                        if (user == null)
                        {
                            _logger.LogWarning(""User not found: {UserId}"", id);
                            return null;
                        }

                        return user;
                    }

                    public async Task<IEnumerable<User>> GetAllUsersAsync()
                    {
                        _logger.LogInformation(""Getting all users"");
                        return await _repository.GetAllAsync();
                    }
                }
            }";
        
        var request = new CompressContextRequest(complexCode, CompressionStrategy.CodeStructure, 0.7);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/compress", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<CompressResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.CompressedContent);
        Assert.True(result.CompressedLength < result.OriginalLength, 
            $"Expected compressed length ({result.CompressedLength}) to be less than original ({result.OriginalLength})");
        
        // Verify that method bodies were removed but signatures preserved
        Assert.DoesNotContain("throw new ArgumentException", result.CompressedContent);
        Assert.DoesNotContain("_logger.LogInformation", result.CompressedContent);
        Assert.Contains("public async Task<User> GetUserByIdAsync(Guid id)", result.CompressedContent);
        Assert.Contains("public async Task<IEnumerable<User>> GetAllUsersAsync()", result.CompressedContent);
    }

    private record CompressResponse(
        string CompressedContent,
        int OriginalLength,
        int CompressedLength,
        double ActualRatio,
        string StrategyUsed);

    /// <summary>
    /// Factory customizada para testes de compressao.
    /// </summary>
    public class CompressWebAppFactory : WebApplicationFactory<Program>
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
                // Remove EF Core registrations
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

                // Register InMemory DbContext
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

                // Register stub IEmbeddingService
                services.AddSingleton<IEmbeddingService, StubEmbeddingService>();
            });
        }
    }

    /// <summary>
    /// Stub para IEmbeddingService usado nos testes de integracao.
    /// </summary>
    private class StubEmbeddingService : IEmbeddingService
    {
        public Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Vector?>(null);
        }
    }
}
