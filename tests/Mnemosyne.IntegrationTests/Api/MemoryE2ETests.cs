using Microsoft.EntityFrameworkCore;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;
using Mnemosyne.Infrastructure.Persistence;
using Mnemosyne.IntegrationTests.Fixtures;

namespace Mnemosyne.IntegrationTests.Api;

[Trait("Category", "E2E")]
[Trait("TestSuite", "Memory")]
public class MemoryE2ETests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public MemoryE2ETests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "E2E - cria memoria e recupera do banco")]
    public async Task CreateMemory_RetrievesFromDatabase()
    {
        await using var context = new MnemosyneDbContext(_fixture.Options);

        var memory = MemoryEntity.Create("Test memory content", MemoryType.Note);
        context.Memories.Add(memory);
        await context.SaveChangesAsync();

        var retrieved = await context.Memories.FirstOrDefaultAsync(m => m.Id == memory.Id);

        Assert.NotNull(retrieved);
        Assert.Equal("Test memory content", retrieved.Content);
        Assert.Equal(MemoryType.Note, retrieved.Type);
    }

    [Fact(DisplayName = "E2E - cria múltiplas memórias e busca retorna todas")]
    public async Task CreateMultipleMemories_SearchReturnsAll()
    {
        await using var context = new MnemosyneDbContext(_fixture.Options);

        var memory1 = MemoryEntity.Create("First memory", MemoryType.Note);
        var memory2 = MemoryEntity.Create("Second memory", MemoryType.Decision);

        context.Memories.AddRange(memory1, memory2);
        await context.SaveChangesAsync();

        var memories = await context.Memories.ToListAsync();

        Assert.Equal(2, memories.Count);
    }

    [Fact(DisplayName = "E2E - memória persiste com timestamps corretos")]
    public async Task Memory_PersistsWithCorrectTimestamps()
    {
        await using var context = new MnemosyneDbContext(_fixture.Options);

        var beforeCreate = DateTime.UtcNow;
        var memory = MemoryEntity.Create("Timestamp test", MemoryType.Context);
        context.Memories.Add(memory);
        await context.SaveChangesAsync();
        var afterCreate = DateTime.UtcNow;

        var retrieved = await context.Memories.FirstAsync(m => m.Id == memory.Id);

        Assert.True(retrieved.CreatedAt >= beforeCreate && retrieved.CreatedAt <= afterCreate);
        Assert.Null(retrieved.UpdatedAt);
    }

    [Fact(DisplayName = "E2E - busca retorna memórias ordenadas por CreatedAt")]
    public async Task Search_ReturnsMemoriesOrderedByCreatedAt()
    {
        await using var context = new MnemosyneDbContext(_fixture.Options);

        var memory1 = MemoryEntity.Create("Older memory", MemoryType.Note);
        var memory2 = MemoryEntity.Create("Newer memory", MemoryType.Note);

        context.Memories.Add(memory1);
        await context.SaveChangesAsync();
        await Task.Delay(100);
        context.Memories.Add(memory2);
        await context.SaveChangesAsync();

        var memories = await context.Memories
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        Assert.Equal(memory2.Id, memories.First().Id);
        Assert.Equal(memory1.Id, memories.Last().Id);
    }
}
