using Mnemosyne.Domain.Entities;

namespace Mnemosyne.Domain.Interfaces;

public interface IUserRepository
{
    Task<UserEntity?> GetByApiKeyHashAsync(string apiKeyHash, CancellationToken cancellationToken);
}