using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Domain.Interfaces;

public interface IProjectRepository
{
    Task<ProjectEntity> AddAsync(ProjectEntity project, CancellationToken cancellationToken);
    Task<ProjectEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProjectEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<ProjectEntity> UpdateAsync(ProjectEntity project, CancellationToken cancellationToken);
    Task DeleteAsync(ProjectEntity project, CancellationToken cancellationToken);
}