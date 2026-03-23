using Mnemosyne.Domain.Enums;
using Pgvector;

namespace Mnemosyne.Domain.Entities;

public class MemoryEntity
{
    public Guid Id { get; private set; }
    public string Content { get; private set; } = null!;
    public MemoryType Type { get; private set; }
    public Vector? Embedding { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }

    private MemoryEntity() { }

    public void SetEmbedding(Vector? embedding)
    {
        Embedding = embedding;
        UpdatedAt = DateTime.UtcNow;
    }

    public static MemoryEntity Create(string content, MemoryType type)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content cannot be empty", nameof(content));
        }

        return new MemoryEntity
        {
            Id = Guid.NewGuid(),
            Content = content,
            Type = type,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
