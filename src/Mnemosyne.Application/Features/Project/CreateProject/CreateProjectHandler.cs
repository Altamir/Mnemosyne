using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Project.CreateProject;

public class CreateProjectHandler
{
    private readonly IProjectRepository _projectRepository;

    public CreateProjectHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectEntity> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Name cannot be empty", nameof(command.Name));
        }

        if (command.UserId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty", nameof(command.UserId));
        }

        var project = ProjectEntity.Create(command.Name, command.UserId, command.Description);
        await _projectRepository.AddAsync(project, cancellationToken);
        return project;
    }
}