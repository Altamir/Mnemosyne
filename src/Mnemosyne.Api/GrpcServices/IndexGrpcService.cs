using Grpc.Core;
using Mnemosyne.Api.Grpc;
using Mnemosyne.Application.Features.Index.StartProjectIndex;
using Mnemosyne.Application.Features.Index.GetIndexStatus;

namespace Mnemosyne.Api.GrpcServices;

public class IndexGrpcService : IndexService.IndexServiceBase
{
    private readonly StartProjectIndexHandler _startHandler;
    private readonly GetIndexStatusHandler _statusHandler;

    public IndexGrpcService(
        StartProjectIndexHandler startHandler,
        GetIndexStatusHandler statusHandler)
    {
        _startHandler = startHandler;
        _statusHandler = statusHandler;
    }

    public override async Task<StartIndexResponse> StartIndex(StartIndexRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProjectId, out var projectId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid project ID format"));
        }

        // TODO: Adapt handler to work with gRPC
        var response = new StartIndexResponse
        {
            JobId = Guid.NewGuid().ToString(),
            Status = "Pending",
            Message = "Index job started successfully"
        };

        return await Task.FromResult(response);
    }

    public override async Task<GetIndexStatusResponse> GetIndexStatus(GetIndexStatusRequest request, ServerCallContext context)
    {
        if (!Guid.TryParse(request.ProjectId, out var projectId))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid project ID format"));
        }

        // TODO: Adapt handler to work with gRPC
        var response = new GetIndexStatusResponse
        {
            JobId = Guid.NewGuid().ToString(),
            Status = "Completed",
            TotalMemories = 0,
            ProcessedMemories = 0
        };

        return await Task.FromResult(response);
    }
}
