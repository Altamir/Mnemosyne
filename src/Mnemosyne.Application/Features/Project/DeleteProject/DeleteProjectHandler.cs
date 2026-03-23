using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Project.DeleteProject;

public class DeleteProjectHandler
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty", nameof(command.Id));
        }

        var project = await _projectRepository.GetByIdAsync(command.Id, cancellationToken);
        if (project is null)
        {
            throw new KeyNotFoundException($"Project with id '{command.Id}' not found");
        }

        await _projectRepository.DeleteAsync(project, cancellationToken);
    }
}
