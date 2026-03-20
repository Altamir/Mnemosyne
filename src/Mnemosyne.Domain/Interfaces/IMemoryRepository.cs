using Mnemosyne.Domain.Entities;
using Pgvector;

namespace Mnemosyne.Domain.Interfaces;

public interface IMemoryRepository
{
    Task<MemoryEntity> AddAsync(MemoryEntity memory, CancellationToken cancellationToken);
    Task<MemoryEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<MemoryEntity>> SearchByEmbeddingAsync(Vector queryEmbedding, CancellationToken cancellationToken, int topK);
}
