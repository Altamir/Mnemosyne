using Microsoft.EntityFrameworkCore;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.Infrastructure.Repositories;

namespace Mnemosyne.UnitTests.Infrastructure.Repositories;

public class UserRepositoryTests
{
    private MnemosyneDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MnemosyneDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new MnemosyneDbContext(options, modelBuilder =>
        {
            modelBuilder.Entity<MemoryEntity>(entity =>
            {
                entity.Ignore(e => e.Embedding);
            });
        });
    }

    [Fact(DisplayName = "API Key valida retorna usuario correspondente")]
    [Trait("Layer", "Infrastructure - Repository")]
    public async Task ValidApiKey_Executed_ReturnsMatchingUser()
    {
        // Arrange
        await using var context = CreateDbContext();
        var rawApiKey = "test-api-key-12345";
        var user = UserEntity.Create(rawApiKey, "user@test.com");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByApiKeyAsync(rawApiKey, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact(DisplayName = "API Key invalida retorna null")]
    [Trait("Layer", "Infrastructure - Repository")]
    public async Task InvalidApiKey_Executed_ReturnsNull()
    {
        // Arrange
        await using var context = CreateDbContext();
        var user = UserEntity.Create("correct-api-key", "user@test.com");
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByApiKeyAsync("wrong-api-key", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "Banco vazio retorna null")]
    [Trait("Layer", "Infrastructure - Repository")]
    public async Task EmptyDatabase_Executed_ReturnsNull()
    {
        // Arrange
        await using var context = CreateDbContext();
        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByApiKeyAsync("any-key", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "Multiplos usuarios retorna apenas o correspondente")]
    [Trait("Layer", "Infrastructure - Repository")]
    public async Task MultipleUsers_Executed_ReturnsCorrectUser()
    {
        // Arrange
        await using var context = CreateDbContext();
        var targetApiKey = "target-api-key";
        var user1 = UserEntity.Create("other-api-key-1", "user1@test.com");
        var user2 = UserEntity.Create(targetApiKey, "user2@test.com");
        var user3 = UserEntity.Create("other-api-key-3", "user3@test.com");
        context.Users.AddRange(user1, user2, user3);
        await context.SaveChangesAsync();

        var repository = new UserRepository(context);

        // Act
        var result = await repository.GetByApiKeyAsync(targetApiKey, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user2.Id, result.Id);
        Assert.Equal("user2@test.com", result.Email);
    }
}
