using Microsoft.Extensions.Logging.Abstractions;
using Mnemosyne.Infrastructure.AI;

namespace Mnemosyne.UnitTests.Infrastructure.AI;

public class OpenAiEmbeddingServiceTests
{
    [Fact(DisplayName = "GenerateEmbedding - Texto vazio lanca ArgumentException")]
    [Trait("Layer", "Infrastructure - AI")]
    public async Task EmptyText_GenerateEmbedding_ThrowsArgumentException()
    {
        // Arrange
        // EmbeddingClient requires valid API credentials, but we only test input validation
        // which happens before the API call. Use a client with dummy credentials.
        var service = CreateServiceWithDummyClient();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GenerateEmbeddingAsync(string.Empty, CancellationToken.None));
    }

    [Fact(DisplayName = "GenerateEmbedding - Texto whitespace lanca ArgumentException")]
    [Trait("Layer", "Infrastructure - AI")]
    public async Task WhitespaceText_GenerateEmbedding_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateServiceWithDummyClient();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GenerateEmbeddingAsync("   ", CancellationToken.None));
    }

    [Fact(DisplayName = "GenerateEmbedding - Texto null lanca ArgumentException")]
    [Trait("Layer", "Infrastructure - AI")]
    public async Task NullText_GenerateEmbedding_ThrowsArgumentException()
    {
        // Arrange
        var service = CreateServiceWithDummyClient();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GenerateEmbeddingAsync(null!, CancellationToken.None));
    }

    private static OpenAiEmbeddingService CreateServiceWithDummyClient()
    {
        // Create an EmbeddingClient with a dummy API key.
        // Tests that validate input will throw before reaching the API.
        var openAiClient = new OpenAI.OpenAIClient("sk-dummy-key-for-tests");
        var embeddingClient = openAiClient.GetEmbeddingClient("text-embedding-3-small");
        var logger = NullLogger<OpenAiEmbeddingService>.Instance;
        return new OpenAiEmbeddingService(embeddingClient, logger);
    }
}
