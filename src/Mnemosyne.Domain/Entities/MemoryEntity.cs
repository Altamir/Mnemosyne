using Mnemosyne.Domain.Enums;

namespace Mnemosyne.Domain.Entities;

public class MemoryEntity
{
    public Guid Id { get; private set; }
    public string Content { get; private set; } = null!;
    public MemoryType Type { get; private set; }
    public DateTime CreatedAt { get; init; }

    private MemoryEntity() { }

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
            CreatedAt = DateTime.UtcNow
        };
    }
}
