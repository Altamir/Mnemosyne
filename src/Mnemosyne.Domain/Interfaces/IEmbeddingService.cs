using Pgvector;

namespace Mnemosyne.Domain.Interfaces;

public interface IEmbeddingService
{
    Task<Vector?> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
}
