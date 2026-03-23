using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken);
}