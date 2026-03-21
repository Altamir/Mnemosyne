using Grpc.Core;
using Mnemosyne.Api.Grpc;
using Mnemosyne.Application.Features.Memory.SearchMemory;

namespace Mnemosyne.Api.GrpcServices;

public class SearchGrpcService : SearchService.SearchServiceBase
{
    private readonly SearchMemoryHandler _handler;

    public SearchGrpcService(SearchMemoryHandler handler)
    {
        _handler = handler;
    }

    public override async Task<SearchResponse> Search(SearchRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Query cannot be empty"));
        }

        // For now, return an empty response since SearchMemoryHandler may need adaptation
        // TODO: Adapt SearchMemoryHandler to work with gRPC request/response
        var response = new SearchResponse
        {
            TotalCount = 0
        };

        return await Task.FromResult(response);
    }
}
