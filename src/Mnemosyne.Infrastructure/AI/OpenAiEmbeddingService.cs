using Mnemosyne.Domain.Interfaces;
using OpenAI;
using OpenAI.Embeddings;
using Pgvector;

namespace Mnemosyne.Infrastructure.AI;

public class OpenAiEmbeddingService : IEmbeddingService
{
    private readonly EmbeddingClient _embeddingClient;

    public OpenAiEmbeddingService(EmbeddingClient embeddingClient)
    {
        _embeddingClient = embeddingClient;
    }

    public async Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }

        var result = await _embeddingClient.GenerateEmbeddingAsync(text, null, cancellationToken);
        var embedding = result.Value;
        
        if (embedding == null)
        {
            return null;
        }

        var vector = embedding.ToFloats();
        return new Vector(vector.ToArray());
    }
}
