using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Memory.CreateMemory;

public class CreateMemoryHandler
{
    private readonly IMemoryRepository _repository;
    private readonly IEmbeddingService _embeddingService;

    public CreateMemoryHandler(IMemoryRepository repository, IEmbeddingService embeddingService)
    {
        _repository = repository;
        _embeddingService = embeddingService;
    }

    public async Task<MemoryEntity> Handle(CreateMemoryCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Content))
        {
            throw new ArgumentException("Content cannot be empty", nameof(command.Content));
        }

        var memory = MemoryEntity.Create(command.Content, command.Type);
        var embedding = await _embeddingService.GenerateEmbeddingAsync(command.Content, cancellationToken);
        memory.SetEmbedding(embedding);
        return await _repository.AddAsync(memory, cancellationToken);
    }
}
