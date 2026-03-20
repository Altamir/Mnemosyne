using Mnemosyne.Domain.Enums;

namespace Mnemosyne.Application.Features.Memory.CreateMemory;

public record CreateMemoryCommand(string Content, MemoryType Type);
