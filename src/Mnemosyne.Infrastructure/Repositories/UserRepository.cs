using Microsoft.EntityFrameworkCore;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Persistence;

namespace Mnemosyne.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MnemosyneDbContext _context;

    public UserRepository(MnemosyneDbContext context)
    {
        _context = context;
    }

    public async Task<UserEntity?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken)
    {
        var users = await _context.Users.ToListAsync(cancellationToken);

        return users.FirstOrDefault(u => u.ValidateApiKey(apiKey));
    }

    public async Task<UserEntity> AddAsync(UserEntity user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }
}
