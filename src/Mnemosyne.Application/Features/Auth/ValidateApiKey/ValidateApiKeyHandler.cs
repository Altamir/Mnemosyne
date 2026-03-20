using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Auth.ValidateApiKey;

public class ValidateApiKeyHandler
{
    private readonly IUserRepository _userRepository;

    public ValidateApiKeyHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserEntity?> Handle(ValidateApiKeyQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.ApiKey))
        {
            throw new ArgumentException("API Key cannot be empty", nameof(query.ApiKey));
        }

        var user = await _userRepository.GetByApiKeyHashAsync(query.ApiKey, cancellationToken);
        return user;
    }
}