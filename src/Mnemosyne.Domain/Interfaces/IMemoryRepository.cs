using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Domain.Interfaces;

public interface IMemoryRepository
{
    Task<MemoryEntity> AddAsync(MemoryEntity memory, CancellationToken cancellationToken);
    Task<MemoryEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
