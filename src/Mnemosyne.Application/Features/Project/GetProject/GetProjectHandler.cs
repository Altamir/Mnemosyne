using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Project.GetProject;

public class GetProjectHandler
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectEntity> Handle(GetProjectQuery query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty", nameof(query.Id));
        }

        var project = await _projectRepository.GetByIdAsync(query.Id, cancellationToken);
        if (project is null)
        {
            throw new KeyNotFoundException($"Project with id '{query.Id}' not found");
        }

        return project;
    }
}
