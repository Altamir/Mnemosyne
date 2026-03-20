using Microsoft.EntityFrameworkCore;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.Infrastructure.Persistence.Migrations;

namespace Mnemosyne.IntegrationTests.Persistence;

public class MigrationTests
{
    [Fact(DisplayName = "Migration - Historico de migrations deve conter InitialMemorySchema")]
    [Trait("Layer", "Infrastructure - Persistence")]
    public void Migration_InitialMemorySchema_ShouldExist()
    {
        var migrationType = typeof(MnemosyneDbContext).Assembly
            .GetType("Mnemosyne.Infrastructure.Persistence.Migrations.InitialMemorySchema");

        Assert.NotNull(migrationType);
    }

    [Fact(DisplayName = "Migration - DbContext consegue criar banco com configuracao")]
    [Trait("Layer", "Infrastructure - Persistence")]
    public void DbContext_CanCreateDatabaseWithConfiguration()
    {
        var options = new DbContextOptionsBuilder<MnemosyneDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new MnemosyneDbContext(options, modelBuilder =>
        {
            modelBuilder.Entity<Domain.Entities.MemoryEntity>(entity =>
            {
                entity.Ignore(e => e.Embedding);
            });
        });

        Assert.True(context.Database.CanConnect());
    }

    [Fact(DisplayName = "Migration - ModelSnapshot esta presente")]
    [Trait("Layer", "Infrastructure - Persistence")]
    public void ModelSnapshot_ShouldBePresent()
    {
        var snapshotType = typeof(MnemosyneDbContext).Assembly
            .GetType("Mnemosyne.Infrastructure.Persistence.Migrations.MnemosyneDbContextModelSnapshot");

        Assert.NotNull(snapshotType);
    }
}
