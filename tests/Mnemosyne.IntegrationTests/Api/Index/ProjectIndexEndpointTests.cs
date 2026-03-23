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

namespace Mnemosyne.IntegrationTests.Api.Index;

public class ProjectIndexEndpointTests : IClassFixture<ProjectIndexEndpointTests.IndexWebAppFactory>, IAsyncLifetime
{
    private readonly IndexWebAppFactory _factory;
    private HttpClient _client = null!;
    private const string ValidApiKey = "test-api-key-index";
    private const string TestEmail = "index-test@mnemosyne.dev";
    private Guid _testUserId;
    private Guid _testProjectId;

    public ProjectIndexEndpointTests(IndexWebAppFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MnemosyneDbContext>();
        await context.Database.EnsureCreatedAsync();

        // Clear existing data
        context.ProjectIndexJobs.RemoveRange(context.ProjectIndexJobs);
        context.Projects.RemoveRange(context.Projects);
        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        // Seed test user
        var user = UserEntity.Create(ValidApiKey, TestEmail);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        _testUserId = user.Id;

        // Seed test project
        var project = ProjectEntity.Create("Index Test Project", _testUserId);
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        _testProjectId = project.Id;

        _client.DefaultRequestHeaders.Add("X-Api-Key", ValidApiKey);
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    #region Start Index

    [Fact(DisplayName = "Start Index - Projeto existente retorna 202 Accepted")]
    [Trait("Layer", "Integration - Api")]
    public async Task ExistingProject_StartIndex_ReturnsAccepted()
    {
        // Arrange
        var request = new { UserId = _testUserId };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/projects/{_testProjectId}/index", request);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

        var job = await response.Content.ReadFromJsonAsync<IndexJobResponse>();
        Assert.NotNull(job);
        Assert.Equal(_testProjectId, job.ProjectId);
        Assert.Equal("Pending", job.Status.ToString());
    }

    [Fact(DisplayName = "Start Index - Projeto inexistente retorna 404 NotFound")]
    [Trait("Layer", "Integration - Api")]
    public async Task NonExistentProject_StartIndex_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new { UserId = _testUserId };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/projects/{nonExistentId}/index", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Start Index - Job ja em progresso retorna 409 Conflict")]
    [Trait("Layer", "Integration - Api")]
    public async Task JobAlreadyInProgress_StartIndex_ReturnsConflict()
    {
        // Arrange - start indexing first
        var request = new { UserId = _testUserId };
        await _client.PostAsJsonAsync($"/api/v1/projects/{_testProjectId}/index", request);

        // Act - try to start again
        var response = await _client.PostAsJsonAsync($"/api/v1/projects/{_testProjectId}/index", request);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact(DisplayName = "Start Index - UserId vazio retorna 400 BadRequest")]
    [Trait("Layer", "Integration - Api")]
    public async Task EmptyUserId_StartIndex_ReturnsBadRequest()
    {
        // Arrange
        var request = new { UserId = Guid.Empty };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/projects/{_testProjectId}/index", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Get Index Status

    [Fact(DisplayName = "Get Index Status - Projeto com job retorna 200 Ok")]
    [Trait("Layer", "Integration - Api")]
    public async Task ProjectWithJob_GetStatus_ReturnsOk()
    {
        // Arrange - start indexing first
        var request = new { UserId = _testUserId };
        await _client.PostAsJsonAsync($"/api/v1/projects/{_testProjectId}/index", request);

        // Act
        var response = await _client.GetAsync($"/api/v1/projects/{_testProjectId}/index/status");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var status = await response.Content.ReadFromJsonAsync<IndexStatusResponse>();
        Assert.NotNull(status);
        Assert.Equal(_testProjectId, status.ProjectId);
    }

    [Fact(DisplayName = "Get Index Status - Projeto sem job retorna 404 NotFound")]
    [Trait("Layer", "Integration - Api")]
    public async Task ProjectWithoutJob_GetStatus_ReturnsNotFound()
    {
        // Arrange - use a project with no index job (create a fresh project)
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MnemosyneDbContext>();
        var newProject = ProjectEntity.Create("No Index Project", _testUserId);
        context.Projects.Add(newProject);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/projects/{newProject.Id}/index/status");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Auth Middleware

    [Fact(DisplayName = "Index Endpoints - Sem API Key retorna 401 Unauthorized")]
    [Trait("Layer", "Integration - Api")]
    public async Task NoApiKey_IndexEndpoint_ReturnsUnauthorized()
    {
        // Arrange
        var clientWithoutKey = _factory.CreateClient();

        // Act
        var response = await clientWithoutKey.GetAsync($"/api/v1/projects/{_testProjectId}/index/status");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        clientWithoutKey.Dispose();
    }

    #endregion

    private record IndexJobResponse(Guid Id, Guid ProjectId, IndexStatus Status, int TotalMemories, int ProcessedMemories, string? ErrorMessage);
    private record IndexStatusResponse(Guid ProjectId, IndexStatus Status, int TotalMemories, int ProcessedMemories, string? ErrorMessage);

    /// <summary>
    /// Factory customizada para testes de indexacao.
    /// Substitui DbContext por InMemory e registra stubs para servicos ausentes.
    /// </summary>
    public class IndexWebAppFactory : WebApplicationFactory<Program>
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
    /// </summary>
    private class StubEmbeddingService : IEmbeddingService
    {
        public Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Vector?>(null);
        }
    }
}
