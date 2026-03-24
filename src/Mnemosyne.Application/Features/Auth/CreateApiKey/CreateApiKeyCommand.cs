using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Auth.CreateApiKey;

public record CreateApiKeyCommand(string Email);

public record CreateApiKeyResult(Guid UserId, string ApiKey, string Email, DateTime CreatedAt);

public class CreateApiKeyHandler
{
    private readonly IUserRepository _userRepository;

    public CreateApiKeyHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CreateApiKeyResult> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(command.Email));
        }

        // Generate a secure random API key
        var apiKey = GenerateSecureApiKey();
        
        var user = UserEntity.Create(apiKey, command.Email);
        await _userRepository.AddAsync(user, cancellationToken);

        return new CreateApiKeyResult(user.Id, apiKey, user.Email, user.CreatedAt);
    }

    private static string GenerateSecureApiKey()
    {
        // Generate a 32-byte random key and convert to base64
        var bytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
