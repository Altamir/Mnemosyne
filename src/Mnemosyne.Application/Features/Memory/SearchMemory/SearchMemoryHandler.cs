using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Memory.SearchMemory;

public class SearchMemoryHandler
{
    private readonly IMemoryRepository _repository;

    public SearchMemoryHandler(IMemoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MemoryEntity>> Handle(SearchMemoryQuery query, CancellationToken cancellationToken)
    {
        var queryEmbedding = new Pgvector.Vector(Array.Empty<float>());
        return await _repository.SearchByEmbeddingAsync(queryEmbedding, cancellationToken, query.TopK);
    }
}
