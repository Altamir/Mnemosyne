using Mnemosyne.Application.Features.Memory.SearchMemory;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Queries.Memory;

public class SearchMemoryHandlerTests
{
    private readonly Mock<IMemoryRepository> _repositoryMock;
    private readonly SearchMemoryHandler _handler;

    public SearchMemoryHandlerTests()
    {
        _repositoryMock = new Mock<IMemoryRepository>();
        _handler = new SearchMemoryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ReturnsMemories()
    {
        var memories = new List<MemoryEntity>
        {
            MemoryEntity.Create("content one", MemoryType.Note),
            MemoryEntity.Create("content two", MemoryType.Decision)
        };

        _repositoryMock
            .Setup(r => r.SearchByEmbeddingAsync(It.IsAny<Pgvector.Vector>(), default, 10))
            .ReturnsAsync(memories);

        var query = new SearchMemoryQuery("test query", 10);

        var results = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, results.Count);
        Assert.Equal("content one", results[0].Content);
        Assert.Equal("content two", results[1].Content);
    }

    [Fact]
    public async Task Handle_WithTopK_ReturnsLimitedResults()
    {
        var memories = new List<MemoryEntity>
        {
            MemoryEntity.Create("first", MemoryType.Note)
        };

        _repositoryMock
            .Setup(r => r.SearchByEmbeddingAsync(It.IsAny<Pgvector.Vector>(), default, 1))
            .ReturnsAsync(memories);

        var query = new SearchMemoryQuery("test", 1);

        var results = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(results);
    }

    [Fact]
    public async Task Handle_WithNoResults_ReturnsEmptyList()
    {
        _repositoryMock
            .Setup(r => r.SearchByEmbeddingAsync(It.IsAny<Pgvector.Vector>(), default, 10))
            .ReturnsAsync(new List<MemoryEntity>());

        var query = new SearchMemoryQuery("nonexistent", 10);

        var results = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(results);
    }
}
