using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.AI;
using OpenAI;
using OpenAI.Embeddings;

namespace Mnemosyne.IntegrationTests.Infrastructure.AI;

public class OpenAiEmbeddingServiceTests : IAsyncLifetime
{
    private ServiceProvider? _serviceProvider;
    private IEmbeddingService? _embeddingService;

    public async Task InitializeAsync()
    {
        // Get OpenAI API key from environment variable (for real integration tests)
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "sk-dummy-key-for-ci";
        
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder => builder.AddConsole());
        
        // Configure services as they would be in Program.cs
        services.AddSingleton(new OpenAIClient(apiKey));
        services.AddSingleton<EmbeddingClient>(sp => 
        {
            var client = sp.GetRequiredService<OpenAIClient>();
            return client.GetEmbeddingClient("text-embedding-3-small");
        });
        services.AddSingleton<IEmbeddingService, OpenAiEmbeddingService>();
        
        _serviceProvider = services.BuildServiceProvider();
        _embeddingService = _serviceProvider.GetRequiredService<IEmbeddingService>();
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider != null)
        {
            await _serviceProvider.DisposeAsync();
        }
    }

    [Fact(DisplayName = "DI Registration - IEmbeddingService resolve como OpenAiEmbeddingService")]
    [Trait("Layer", "Infrastructure - AI")]
    public void DiRegistration_Executed_ResolvesCorrectImplementation()
    {
        // Arrange & Act
        var service = _serviceProvider!.GetRequiredService<IEmbeddingService>();

        // Assert
        Assert.NotNull(service);
        Assert.IsType<OpenAiEmbeddingService>(service);
    }

    [Fact(DisplayName = "DI Registration - EmbeddingClient e resolvido corretamente")]
    [Trait("Layer", "Infrastructure - AI")]
    public void DiRegistration_Executed_EmbeddingClientResolved()
    {
        // Arrange & Act
        var client = _serviceProvider!.GetRequiredService<EmbeddingClient>();

        // Assert
        Assert.NotNull(client);
    }

    [Fact(DisplayName = "GenerateEmbedding - Texto vazio lanca ArgumentException")]
    [Trait("Layer", "Infrastructure - AI")]
    public async Task EmptyText_GenerateEmbedding_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _embeddingService!.GenerateEmbeddingAsync(string.Empty, CancellationToken.None));
    }

    [Fact(DisplayName = "GenerateEmbedding - Texto whitespace lanca ArgumentException")]
    [Trait("Layer", "Infrastructure - AI")]
    public async Task WhitespaceText_GenerateEmbedding_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _embeddingService!.GenerateEmbeddingAsync("   ", CancellationToken.None));
    }

    [Fact(DisplayName = "GenerateEmbedding - Texto null lanca ArgumentException")]
    [Trait("Layer", "Infrastructure - AI")]
    public async Task NullText_GenerateEmbedding_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _embeddingService!.GenerateEmbeddingAsync(null!, CancellationToken.None));
    }
}
