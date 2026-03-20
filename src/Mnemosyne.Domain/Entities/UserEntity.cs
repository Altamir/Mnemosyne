namespace Mnemosyne.Domain.Entities;

public class UserEntity
{
    public Guid Id { get; private set; }
    public string ApiKeyHash { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public DateTime CreatedAt { get; init; }

    private UserEntity() { }

    public static UserEntity Create(string apiKey, string email)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API Key cannot be empty", nameof(apiKey));
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        return new UserEntity
        {
            Id = Guid.NewGuid(),
            ApiKeyHash = BCrypt.Net.BCrypt.HashPassword(apiKey),
            Email = email,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool ValidateApiKey(string apiKey)
    {
        return BCrypt.Net.BCrypt.Verify(apiKey, ApiKeyHash);
    }
}