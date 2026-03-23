using Grpc.Core;
using Mnemosyne.Api.Grpc;
using Mnemosyne.Application.Features.Compress.CompressContext;
using Mnemosyne.Domain.Enums;

namespace Mnemosyne.Api.GrpcServices;

public class CompressGrpcService : CompressService.CompressServiceBase
{
    private readonly CompressContextHandler _handler;

    public CompressGrpcService(CompressContextHandler handler)
    {
        _handler = handler;
    }

    public override async Task<CompressResponse> Compress(CompressRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Content cannot be empty"));
        }

        if (!Enum.TryParse<CompressionStrategy>(request.Strategy, out var strategy))
        {
            strategy = CompressionStrategy.CodeStructure;
        }

        var command = new CompressContextCommand(
            request.Content,
            strategy,
            request.TargetRatio);

        var result = await _handler.Handle(command, context.CancellationToken);

        return new CompressResponse
        {
            CompressedContent = result.CompressedContent,
            OriginalLength = result.OriginalLength,
            CompressedLength = result.CompressedLength,
            ActualRatio = result.ActualRatio,
            StrategyUsed = result.StrategyUsed
        };
    }
}
