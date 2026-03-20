using AutoFixture;
using Mnemosyne.Application.Features.Memory.CreateMemory;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Commands;

public class CreateMemoryHandlerTests
{
    private readonly Mock<IMemoryRepository> _repositoryMock;
    private readonly CreateMemoryHandler _handler;
    private readonly Fixture _fixture;

    public CreateMemoryHandlerTests()
    {
        _repositoryMock = new Mock<IMemoryRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new CreateMemoryHandler(_repositoryMock.Object);
    }

    [Fact(DisplayName = "Criar memória com conteúdo válido retorna memória criada")]
    [Trait("Layer", "Application - Commands")]
    public async Task ValidContent_Executed_ReturnsCreatedMemory()
    {
        // Arrange
        var content = _fixture.Create<string>();
        var memoryType = _fixture.Create<MemoryType>();
        var command = new CreateMemoryCommand(content, memoryType);
        var createdMemory = MemoryEntity.Create(content, memoryType);
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<MemoryEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdMemory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(content, result.Content);
        Assert.Equal(memoryType, result.Type);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MemoryEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Criar memória com conteúdo vazio lança ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmptyContent_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateMemoryCommand("", MemoryType.Note);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MemoryEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Criar memória com whitespace lança ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task WhitespaceContent_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateMemoryCommand("   ", MemoryType.Decision);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MemoryEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory(DisplayName = "Criar memória com todos os tipos succeeds")]
    [Trait("Layer", "Application - Commands")]
    [InlineData(MemoryType.Note)]
    [InlineData(MemoryType.Decision)]
    [InlineData(MemoryType.Preference)]
    [InlineData(MemoryType.Context)]
    public async Task AllMemoryTypes_Executed_ReturnsMemoryWithCorrectType(MemoryType memoryType)
    {
        // Arrange
        var content = _fixture.Create<string>();
        var command = new CreateMemoryCommand(content, memoryType);
        var createdMemory = MemoryEntity.Create(content, memoryType);
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<MemoryEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdMemory);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(memoryType, result.Type);
    }
}
