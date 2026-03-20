using Microsoft.EntityFrameworkCore;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.Infrastructure.Repositories;

public class MemoryRepository : IMemoryRepository
{
    private readonly MnemosyneDbContext _context;

    public MemoryRepository(MnemosyneDbContext context)
    {
        _context = context;
    }

    public async Task<MemoryEntity> AddAsync(MemoryEntity memory, CancellationToken cancellationToken)
    {
        await _context.Memories.AddAsync(memory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return memory;
    }

    public async Task<MemoryEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Memories.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }
}
