namespace Mnemosyne.Domain.Entities;

public class ProjectEntity
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }

    private ProjectEntity() { }

    public static ProjectEntity Create(string name, Guid userId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty", nameof(name));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty", nameof(userId));
        }

        return new ProjectEntity
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}