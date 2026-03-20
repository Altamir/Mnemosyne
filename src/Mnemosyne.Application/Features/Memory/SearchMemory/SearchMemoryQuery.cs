namespace Mnemosyne.Application.Features.Memory.SearchMemory;

public record SearchMemoryQuery(string Query, int TopK = 10);
