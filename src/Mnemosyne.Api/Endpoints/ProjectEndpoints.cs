using Mnemosyne.Application.Features.Project.CreateProject;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Api.Endpoints;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/projects").WithTags("Projects");

        group.MapPost("/", async (CreateProjectRequest request, CreateProjectHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new CreateProjectCommand(request.Name, request.UserId, request.Description);
                var project = await handler.Handle(command, cancellationToken);
                return Results.Created($"/api/v1/projects/{project.Id}", project);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CreateProject")
        .WithSummary("Creates a new project")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
    }
}

public record CreateProjectRequest(string Name, Guid UserId, string? Description = null);