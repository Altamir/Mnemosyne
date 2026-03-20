using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Memory.CreateMemory;

public class CreateMemoryHandler
{
    private readonly IMemoryRepository _repository;

    public CreateMemoryHandler(IMemoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<MemoryEntity> Handle(CreateMemoryCommand command, CancellationToken cancellationToken)
    {
        var memory = MemoryEntity.Create(command.Content, command.Type);
        return await _repository.AddAsync(memory, cancellationToken);
    }
}
