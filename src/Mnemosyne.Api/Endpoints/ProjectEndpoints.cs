using Mnemosyne.Application.Features.Index.GetIndexStatus;
using Mnemosyne.Application.Features.Index.StartProjectIndex;
using Mnemosyne.Application.Features.Project.CreateProject;
using Mnemosyne.Application.Features.Project.DeleteProject;
using Mnemosyne.Application.Features.Project.GetProject;
using Mnemosyne.Application.Features.Project.ListProjects;
using Mnemosyne.Application.Features.Project.UpdateProject;
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

        group.MapGet("/", async ([AsParameters] ListProjectsRequest request, ListProjectsHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var query = new ListProjectsQuery(request.UserId);
                var projects = await handler.Handle(query, cancellationToken);
                return Results.Ok(projects);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("ListProjects")
        .WithSummary("Lists projects for a user")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", async (Guid id, GetProjectHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var query = new GetProjectQuery(id);
                var project = await handler.Handle(query, cancellationToken);
                return Results.Ok(project);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("GetProject")
        .WithSummary("Gets a project by id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", async (Guid id, UpdateProjectRequest request, UpdateProjectHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new UpdateProjectCommand(id, request.Name, request.Description);
                var project = await handler.Handle(command, cancellationToken);
                return Results.Ok(project);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("UpdateProject")
        .WithSummary("Updates a project")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapDelete("/{id:guid}", async (Guid id, DeleteProjectHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new DeleteProjectCommand(id);
                await handler.Handle(command, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("DeleteProject")
        .WithSummary("Deletes a project")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/{id:guid}/index", async (Guid id, StartProjectIndexRequest request, StartProjectIndexHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new StartProjectIndexCommand(id, request.UserId);
                var job = await handler.Handle(command, cancellationToken);
                return Results.Accepted($"/api/v1/projects/{id}/index/status", job);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("StartProjectIndex")
        .WithSummary("Starts async indexing for a project")
        .Produces(StatusCodes.Status202Accepted)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}/index/status", async (Guid id, GetIndexStatusHandler handler, CancellationToken cancellationToken) =>
        {
            var query = new GetIndexStatusQuery(id);
            var result = await handler.Handle(query, cancellationToken);
            if (result == null)
            {
                return Results.NotFound(new { error = "No index job found for this project" });
            }
            return Results.Ok(result);
        })
        .WithName("GetIndexStatus")
        .WithSummary("Gets the index status for a project")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}

public record CreateProjectRequest(string Name, Guid UserId, string? Description = null);
public record UpdateProjectRequest(string Name, string? Description = null);
public record ListProjectsRequest(Guid UserId);
public record StartProjectIndexRequest(Guid UserId);