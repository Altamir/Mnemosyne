using Microsoft.EntityFrameworkCore;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.Infrastructure.Repositories;

public class ProjectIndexJobRepository : IProjectIndexJobRepository
{
    private readonly MnemosyneDbContext _context;

    public ProjectIndexJobRepository(MnemosyneDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectIndexJobEntity> AddAsync(ProjectIndexJobEntity job, CancellationToken cancellationToken)
    {
        await _context.ProjectIndexJobs.AddAsync(job, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return job;
    }

    public async Task<ProjectIndexJobEntity> UpdateAsync(ProjectIndexJobEntity job, CancellationToken cancellationToken)
    {
        _context.ProjectIndexJobs.Update(job);
        await _context.SaveChangesAsync(cancellationToken);
        return job;
    }

    public async Task<ProjectIndexJobEntity?> GetLatestByProjectIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _context.ProjectIndexJobs
            .Where(j => j.ProjectId == projectId)
            .OrderByDescending(j => j.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProjectIndexJobEntity?> GetPendingOrProcessingByProjectIdAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await _context.ProjectIndexJobs
            .Where(j => j.ProjectId == projectId && (j.Status == IndexStatus.Pending || j.Status == IndexStatus.Processing))
            .OrderByDescending(j => j.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectIndexJobEntity>> GetPendingJobsAsync(CancellationToken cancellationToken)
    {
        return await _context.ProjectIndexJobs
            .Where(j => j.Status == IndexStatus.Pending)
            .OrderBy(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
