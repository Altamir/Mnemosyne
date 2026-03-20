using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Application.Features.Index.GetIndexStatus;

public record GetIndexStatusQuery(Guid ProjectId);

public record GetIndexStatusResult(
    Guid ProjectId,
    IndexStatus Status,
    int TotalMemories,
    int ProcessedMemories,
    string? ErrorMessage,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
