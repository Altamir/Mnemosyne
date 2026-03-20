namespace Mnemosyne.Application.Features.Project.UpdateProject;

public record UpdateProjectCommand(Guid Id, string Name, string? Description = null);
