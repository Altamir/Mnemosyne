using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Index.StartProjectIndex;

public class StartProjectIndexHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectIndexJobRepository _indexJobRepository;

    public StartProjectIndexHandler(IProjectRepository projectRepository, IProjectIndexJobRepository indexJobRepository)
    {
        _projectRepository = projectRepository;
        _indexJobRepository = indexJobRepository;
    }

    public async Task<ProjectIndexJobEntity> Handle(StartProjectIndexCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty", nameof(command.UserId));
        }

        var project = await _projectRepository.GetByIdAsync(command.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new KeyNotFoundException("Project not found");
        }

        var existingJob = await _indexJobRepository.GetPendingOrProcessingByProjectIdAsync(command.ProjectId, cancellationToken);
        if (existingJob != null)
        {
            throw new InvalidOperationException("Index operation already in progress for this project");
        }

        var job = ProjectIndexJobEntity.Create(command.ProjectId);
        await _indexJobRepository.AddAsync(job, cancellationToken);
        return job;
    }
}
