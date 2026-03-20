using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Pgvector;

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
        var queryEmbedding = CreateQueryEmbedding(query.Query);
        return await _repository.SearchByEmbeddingAsync(queryEmbedding, cancellationToken, query.TopK);
    }

    private static Vector CreateQueryEmbedding(string? text)
    {
        const int embeddingDimension = 1536;
        var embedding = new float[embeddingDimension];

        if (string.IsNullOrWhiteSpace(text))
        {
            return new Vector(embedding);
        }

        unchecked
        {
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                int index = (c * 31 + i) % embeddingDimension;
                embedding[index] += (c % 31) / 31.0f;
            }
        }

        return new Vector(embedding);
    }
}
