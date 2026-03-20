using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Project.UpdateProject;

public class UpdateProjectHandler
{
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ProjectEntity> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty", nameof(command.Id));
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            throw new ArgumentException("Name cannot be empty", nameof(command.Name));
        }

        var project = await _projectRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
        {
            throw new KeyNotFoundException($"Project with id '{command.Id}' not found");
        }

        project.Update(command.Name, command.Description);
        return await _projectRepository.UpdateAsync(project, cancellationToken);
    }
}
