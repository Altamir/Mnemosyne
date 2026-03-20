using Microsoft.AspNetCore.Mvc.Testing;
using Mnemosyne.Domain.Enums;
using System.Net;
using System.Net.Http.Json;

namespace Mnemosyne.IntegrationTests.Api;

[Trait("Category", "E2E")]
[Trait("TestSuite", "Memory")]
public class MemoryE2ETests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public MemoryE2ETests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact(DisplayName = "E2E - cria memoria via API e recupera do banco")]
    public async Task CreateMemory_ViaApi_ReturnsCreated()
    {
        // Arrange
        var request = new { Content = "E2E test memory", Type = MemoryType.Note };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/memory/", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
    }

    [Fact(DisplayName = "E2E - cria multiplas memorias via API")]
    public async Task CreateMemory_ViaApi_MultipleMemories()
    {
        // Arrange & Act
        var response1 = await _client.PostAsJsonAsync("/api/v1/memory/", 
            new { Content = "First", Type = MemoryType.Note });
        var response2 = await _client.PostAsJsonAsync("/api/v1/memory/", 
            new { Content = "Second", Type = MemoryType.Decision });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, response2.StatusCode);
    }

    [Fact(DisplayName = "E2E - busca memorias via API")]
    public async Task SearchMemory_ViaApi_ReturnsMemories()
    {
        // Arrange - create a memory first
        await _client.PostAsJsonAsync("/api/v1/memory/", 
            new { Content = "Searchable memory", Type = MemoryType.Note });

        // Act
        var response = await _client.GetAsync("/api/v1/memory/search?q=Searchable&topK=10");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
