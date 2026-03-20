using Microsoft.EntityFrameworkCore;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.Infrastructure.Persistence.Configurations;

namespace Mnemosyne.IntegrationTests.Persistence;

public class DbContextMappingTests
{
    private DbContextOptions<MnemosyneDbContext> CreateContextOptions()
    {
        return new DbContextOptionsBuilder<MnemosyneDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private MnemosyneDbContext CreateDbContextForInMemoryTesting()
    {
        var options = CreateContextOptions();
        return new MnemosyneDbContext(options, modelBuilder =>
        {
            modelBuilder.Entity<MemoryEntity>(entity =>
            {
                entity.Ignore(e => e.Embedding);
            });
        });
    }

    [Fact(DisplayName = "MemoryEntity - DbContext consegue adicionar e recuperar entidade")]
    [Trait("Layer", "Infrastructure - Persistence")]
    public async Task MemoryEntity_CanBeAddedAndRetrieved()
    {
        // Arrange
        await using var context = CreateDbContextForInMemoryTesting();
        var memory = MemoryEntity.Create("Test content", MemoryType.Note);

        // Act
        context.Memories.Add(memory);
        await context.SaveChangesAsync();

        // Assert
        var retrieved = await context.Memories.FindAsync(memory.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(memory.Id, retrieved.Id);
        Assert.Equal("Test content", retrieved.Content);
        Assert.Equal(MemoryType.Note, retrieved.Type);
    }

    [Fact(DisplayName = "MemoryEntity - DbContext consegue mapear multiplas entidades")]
    [Trait("Layer", "Infrastructure - Persistence")]
    public async Task MemoryEntity_MultipleEntitiesCanBeRetrieved()
    {
        // Arrange
        await using var context = CreateDbContextForInMemoryTesting();
        var memories = new[]
        {
            MemoryEntity.Create("Content 1", MemoryType.Note),
            MemoryEntity.Create("Content 2", MemoryType.Decision),
            MemoryEntity.Create("Content 3", MemoryType.Preference)
        };

        context.Memories.AddRange(memories);
        await context.SaveChangesAsync();

        // Act
        var allMemories = await context.Memories.ToListAsync();

        // Assert
        Assert.Equal(3, allMemories.Count);
    }

    [Fact(DisplayName = "MemoryEntity - Configuracao de tabela esta correta")]
    [Trait("Layer", "Infrastructure - Persistence")]
    public void MemoryEntity_TableConfiguration_IsCorrect()
    {
        // Arrange & Act
        var configuration = new MemoryEntityConfiguration();

        // Assert
        Assert.Equal(MemoryEntityConfiguration.TableName, "memories");
        Assert.Equal(MemoryEntityConfiguration.SchemaName, "mnemosyne");
        Assert.Equal(MemoryEntityConfiguration.EmbeddingDimension, 1536);
    }
}
