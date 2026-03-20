using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mnemosyne.Application.Features.Memory.CreateMemory;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.Infrastructure.Repositories;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Mnemosyne.IntegrationTests.Api;

public class CreateMemoryEndpointTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly MnemosyneDbContext _context;

    public CreateMemoryEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<MnemosyneDbContext>();
    }

    public void Dispose()
    {
        _scope.Dispose();
        _client.Dispose();
    }

    [Fact(DisplayName = "POST /api/v1/memory - cria memoria com sucesso")]
    [Trait("Layer", "Integration - API")]
    public async Task CreateMemory_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new { Content = "Test memory content", Type = MemoryType.Note };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/memory/", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact(DisplayName = "POST /api/v1/memory - memoria criada persiste no banco")]
    [Trait("Layer", "Integration - API")]
    public async Task CreateMemory_WithValidRequest_PersistsInDatabase()
    {
        // Arrange
        var request = new { Content = "Persist test", Type = MemoryType.Decision };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/memory/", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var location = response.Headers.Location?.ToString();
        Assert.NotNull(location);
        var memoryId = location.Split('/').Last();
        Assert.True(Guid.TryParse(memoryId, out var id));

        var dbMemory = await _context.Memories.FirstOrDefaultAsync(m => m.Id == id);
        Assert.NotNull(dbMemory);
        Assert.Equal("Persist test", dbMemory.Content);
    }

    [Theory(DisplayName = "POST /api/v1/memory - todos os tipos de memoria")]
    [Trait("Layer", "Integration - API")]
    [InlineData(MemoryType.Note)]
    [InlineData(MemoryType.Decision)]
    [InlineData(MemoryType.Preference)]
    [InlineData(MemoryType.Context)]
    public async Task CreateMemory_WithDifferentTypes_ReturnsCreated(MemoryType memoryType)
    {
        // Arrange
        var request = new { Content = "Test content", Type = memoryType };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/memory/", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptors = services
                .Where(d => d.ServiceType == typeof(DbContextOptions<MnemosyneDbContext>)
                         || d.ServiceType.FullName?.Contains("DbContext") == true)
                .ToList();

            foreach (var descriptor in dbContextDescriptors)
            {
                services.Remove(descriptor);
            }

            var npgsqlServices = services
                .Where(d => d.ServiceType.FullName?.Contains("Npgsql") == true)
                .ToList();

            foreach (var service in npgsqlServices)
            {
                services.Remove(service);
            }

            services.AddDbContext<MnemosyneDbContext>((_, options) =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        });

        builder.UseEnvironment("Testing");
    }
}
