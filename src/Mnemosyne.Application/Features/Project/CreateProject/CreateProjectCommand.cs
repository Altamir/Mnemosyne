namespace Mnemosyne.Application.Features.Project.CreateProject;

public record CreateProjectCommand(string Name, Guid UserId, string? Description = null);