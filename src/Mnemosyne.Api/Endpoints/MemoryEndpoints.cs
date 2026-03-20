using Mnemosyne.Application.Features.Memory.CreateMemory;
using Mnemosyne.Application.Features.Memory.SearchMemory;
using Mnemosyne.Domain.Enums;

namespace Mnemosyne.Api.Endpoints;

public static class MemoryEndpoints
{
    public static void MapMemoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/memory").WithTags("Memory");

        group.MapPost("/", async (CreateMemoryRequest request, CreateMemoryHandler handler, CancellationToken cancellationToken) =>
        {
            var command = new CreateMemoryCommand(request.Content, request.Type);
            var memory = await handler.Handle(command, cancellationToken);
            return Results.Created($"/api/v1/memory/{memory.Id}", memory);
        })
        .WithName("CreateMemory")
        .WithSummary("Creates a new memory")
        .Produces(StatusCodes.Status201Created);

        group.MapGet("/search", async (string q, int topK, SearchMemoryHandler handler, CancellationToken cancellationToken) =>
        {
            var query = new SearchMemoryQuery(q ?? "", topK);
            var results = await handler.Handle(query, cancellationToken);
            return Results.Ok(results);
        })
        .WithName("SearchMemory")
        .WithSummary("Searches memories")
        .Produces(StatusCodes.Status200OK);
    }
}

public record CreateMemoryRequest(string Content, MemoryType Type);
