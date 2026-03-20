using AutoFixture;
using Mnemosyne.Application.Features.Memory.CreateMemory;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;
using Mnemosyne.Domain.Interfaces;
using Moq;
using Pgvector;

namespace Mnemosyne.UnitTests.Application.Memory;

public class CreateMemoryEmbeddingTests
{
    private readonly Mock<IMemoryRepository> _repositoryMock;
    private readonly Mock<IEmbeddingService> _embeddingServiceMock;
    private readonly CreateMemoryHandler _handler;
    private readonly Fixture _fixture;

    public CreateMemoryEmbeddingTests()
    {
        _repositoryMock = new Mock<IMemoryRepository>();
        _embeddingServiceMock = new Mock<IEmbeddingService>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new CreateMemoryHandler(_repositoryMock.Object, _embeddingServiceMock.Object);
    }

    [Fact(DisplayName = "Criacao de memoria com conteudo valido gera embedding")]
    [Trait("Layer", "Application - Commands")]
    public async Task ValidContent_Executed_GeneratesEmbedding()
    {
        // Arrange
        var content = _fixture.Create<string>();
        var memoryType = MemoryType.Note;
        var embedding = new Vector(new float[] { 0.1f, 0.2f, 0.3f });
        var command = new CreateMemoryCommand(content, memoryType);
        var createdMemory = MemoryEntity.Create(content, memoryType);

        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(content, It.IsAny<CancellationToken>()))
            .ReturnsAsync(embedding);
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<MemoryEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdMemory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _embeddingServiceMock.Verify(x => x.GenerateEmbeddingAsync(content, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Criacao de memoria com texto vazio lanca ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmptyContent_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateMemoryCommand(string.Empty, MemoryType.Note);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _embeddingServiceMock.Verify(x => x.GenerateEmbeddingAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Embedding service retornando null define embedding como null na memoria")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmbeddingServiceReturnsNull_Executed_SetsNullEmbedding()
    {
        // Arrange
        var content = _fixture.Create<string>();
        var command = new CreateMemoryCommand(content, MemoryType.Note);
        var createdMemory = MemoryEntity.Create(content, MemoryType.Note);

        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(content, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vector?)null);
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<MemoryEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdMemory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _embeddingServiceMock.Verify(x => x.GenerateEmbeddingAsync(content, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Embedding service lancando excecao propaga exceção")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmbeddingServiceThrows_Executed_PropagatesException()
    {
        // Arrange
        var content = _fixture.Create<string>();
        var command = new CreateMemoryCommand(content, MemoryType.Note);

        _embeddingServiceMock
            .Setup(x => x.GenerateEmbeddingAsync(content, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("OpenAI API error"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
