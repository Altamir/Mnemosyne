using Microsoft.EntityFrameworkCore;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly MnemosyneDbContext _context;

    public ProjectRepository(MnemosyneDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectEntity> AddAsync(ProjectEntity project, CancellationToken cancellationToken)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task<ProjectEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _context.Projects
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}