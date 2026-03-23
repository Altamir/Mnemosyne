using Mnemosyne.Application.Features.Compress.CompressContext;
using Mnemosyne.Domain.Enums;

namespace Mnemosyne.Api.Endpoints;

public static class CompressEndpoints
{
    public static void MapCompressEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/v1/compress").WithTags("Compress");

        group.MapPost("/", async (CompressContextRequest request, CompressContextHandler handler, CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new CompressContextCommand(
                    request.Content,
                    request.Strategy,
                    request.TargetRatio);
                var result = await handler.Handle(command, cancellationToken);
                return Results.Ok(new CompressContextResponse(
                    result.CompressedContent,
                    result.OriginalLength,
                    result.CompressedLength,
                    result.ActualRatio,
                    result.StrategyUsed));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("CompressContext")
        .WithSummary("Compresses content using the specified strategy")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
    }
}

public record CompressContextRequest(
    string Content,
    CompressionStrategy Strategy = CompressionStrategy.CodeStructure,
    double TargetRatio = 0.7);

public record CompressContextResponse(
    string CompressedContent,
    int OriginalLength,
    int CompressedLength,
    double ActualRatio,
    string StrategyUsed);
