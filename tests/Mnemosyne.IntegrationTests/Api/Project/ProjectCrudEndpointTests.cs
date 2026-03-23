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

namespace Mnemosyne.IntegrationTests.Api.Project;

public class ProjectCrudEndpointTests : IClassFixture<ProjectCrudEndpointTests.ProjectWebAppFactory>, IAsyncLifetime
{
    private readonly ProjectWebAppFactory _factory;
    private HttpClient _client = null!;
    private const string ValidApiKey = "test-api-key-project-crud";
    private const string TestEmail = "project-test@mnemosyne.dev";
    private Guid _testUserId;

    public ProjectCrudEndpointTests(ProjectWebAppFactory factory)
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
        context.Projects.RemoveRange(context.Projects);
        context.Users.RemoveRange(context.Users);
        await context.SaveChangesAsync();

        // Seed a test user for API key authentication
        var user = UserEntity.Create(ValidApiKey, TestEmail);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        _testUserId = user.Id;

        // Set default API key header for all requests
        _client.DefaultRequestHeaders.Add("X-Api-Key", ValidApiKey);
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        return Task.CompletedTask;
    }

    #region Create Project

    [Fact(DisplayName = "Create Project - Dados validos retorna 201 Created")]
    [Trait("Layer", "Integration - Api")]
    public async Task ValidProject_Create_ReturnsCreated()
    {
        // Arrange
        var request = new { Name = "Test Project", UserId = _testUserId, Description = "A test project" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/projects", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        Assert.NotNull(project);
        Assert.NotEqual(Guid.Empty, project.Id);
        Assert.Equal("Test Project", project.Name);
        Assert.Equal("A test project", project.Description);
        Assert.Equal(_testUserId, project.UserId);
    }

    [Fact(DisplayName = "Create Project - Nome vazio retorna 400 BadRequest")]
    [Trait("Layer", "Integration - Api")]
    public async Task EmptyName_Create_ReturnsBadRequest()
    {
        // Arrange
        var request = new { Name = "", UserId = _testUserId };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/projects", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Get Project

    [Fact(DisplayName = "Get Project - Projeto existente retorna 200 Ok")]
    [Trait("Layer", "Integration - Api")]
    public async Task ExistingProject_Get_ReturnsOk()
    {
        // Arrange - create a project first
        var createRequest = new { Name = "Get Test Project", UserId = _testUserId };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/projects", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        // Act
        var response = await _client.GetAsync($"/api/v1/projects/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        Assert.NotNull(project);
        Assert.Equal(created.Id, project.Id);
        Assert.Equal("Get Test Project", project.Name);
    }

    [Fact(DisplayName = "Get Project - Projeto inexistente retorna 404 NotFound")]
    [Trait("Layer", "Integration - Api")]
    public async Task NonExistentProject_Get_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/projects/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region List Projects

    [Fact(DisplayName = "List Projects - Usuario com projetos retorna 200 Ok com lista")]
    [Trait("Layer", "Integration - Api")]
    public async Task UserWithProjects_List_ReturnsOkWithProjects()
    {
        // Arrange - create two projects
        var request1 = new { Name = "List Project 1", UserId = _testUserId };
        var request2 = new { Name = "List Project 2", UserId = _testUserId };
        await _client.PostAsJsonAsync("/api/v1/projects", request1);
        await _client.PostAsJsonAsync("/api/v1/projects", request2);

        // Act
        var response = await _client.GetAsync($"/api/v1/projects?userId={_testUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectResponse>>();
        Assert.NotNull(projects);
        Assert.True(projects.Count >= 2);
    }

    [Fact(DisplayName = "List Projects - Usuario sem projetos retorna 200 Ok com lista vazia")]
    [Trait("Layer", "Integration - Api")]
    public async Task UserWithoutProjects_List_ReturnsEmptyList()
    {
        // Arrange
        var newUserId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/projects?userId={newUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await response.Content.ReadFromJsonAsync<List<ProjectResponse>>();
        Assert.NotNull(projects);
        Assert.Empty(projects);
    }

    #endregion

    #region Update Project

    [Fact(DisplayName = "Update Project - Dados validos retorna 200 Ok com projeto atualizado")]
    [Trait("Layer", "Integration - Api")]
    public async Task ValidUpdate_Update_ReturnsOkWithUpdatedProject()
    {
        // Arrange - create a project first
        var createRequest = new { Name = "Original Name", UserId = _testUserId, Description = "Original description" };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/projects", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        var updateRequest = new { Name = "Updated Name", Description = "Updated description" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/projects/{created!.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await response.Content.ReadFromJsonAsync<ProjectResponse>();
        Assert.NotNull(project);
        Assert.Equal(created.Id, project.Id);
        Assert.Equal("Updated Name", project.Name);
        Assert.Equal("Updated description", project.Description);
    }

    [Fact(DisplayName = "Update Project - Projeto inexistente retorna 404 NotFound")]
    [Trait("Layer", "Integration - Api")]
    public async Task NonExistentProject_Update_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new { Name = "Updated Name" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/projects/{nonExistentId}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "Update Project - Nome vazio retorna 400 BadRequest")]
    [Trait("Layer", "Integration - Api")]
    public async Task EmptyName_Update_ReturnsBadRequest()
    {
        // Arrange - create a project first
        var createRequest = new { Name = "Project to Update", UserId = _testUserId };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/projects", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        var updateRequest = new { Name = "" };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/projects/{created!.Id}", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Delete Project

    [Fact(DisplayName = "Delete Project - Projeto existente retorna 204 NoContent")]
    [Trait("Layer", "Integration - Api")]
    public async Task ExistingProject_Delete_ReturnsNoContent()
    {
        // Arrange - create a project first
        var createRequest = new { Name = "Project to Delete", UserId = _testUserId };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/projects", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProjectResponse>();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/projects/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the project is actually deleted
        var getResponse = await _client.GetAsync($"/api/v1/projects/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact(DisplayName = "Delete Project - Projeto inexistente retorna 404 NotFound")]
    [Trait("Layer", "Integration - Api")]
    public async Task NonExistentProject_Delete_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/projects/{nonExistentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Auth Middleware

    [Fact(DisplayName = "Project Endpoints - Sem API Key retorna 401 Unauthorized")]
    [Trait("Layer", "Integration - Api")]
    public async Task NoApiKey_AnyEndpoint_ReturnsUnauthorized()
    {
        // Arrange - create a client without API key header
        var clientWithoutKey = _factory.CreateClient();

        // Act
        var response = await clientWithoutKey.GetAsync($"/api/v1/projects?userId={_testUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        clientWithoutKey.Dispose();
    }

    #endregion

    private record ProjectResponse(Guid Id, string Name, string? Description, Guid UserId, DateTime CreatedAt, DateTime UpdatedAt);

    /// <summary>
    /// Factory customizada para testes de CRUD de projetos.
    /// Substitui DbContext por InMemory e registra stubs para servicos ausentes.
    /// </summary>
    public class ProjectWebAppFactory : WebApplicationFactory<Program>
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
