using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Domain.Interfaces;

public interface IProjectIndexJobRepository
{
    Task<ProjectIndexJobEntity> AddAsync(ProjectIndexJobEntity job, CancellationToken cancellationToken);
    Task<ProjectIndexJobEntity> UpdateAsync(ProjectIndexJobEntity job, CancellationToken cancellationToken);
    Task<ProjectIndexJobEntity?> GetLatestByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);
    Task<ProjectIndexJobEntity?> GetPendingOrProcessingByProjectIdAsync(Guid projectId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectIndexJobEntity>> GetPendingJobsAsync(CancellationToken cancellationToken);
}
