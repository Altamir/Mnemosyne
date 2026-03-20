using AutoFixture;
using Mnemosyne.Application.Features.Compress.CompressContext;
using Mnemosyne.Domain.Enums;
using Mnemosyne.Domain.Services;
using Moq;

namespace Mnemosyne.UnitTests.Application.Compress;

public class CompressContextHandlerTests
{
    private readonly Mock<ICompressionStrategy> _strategyMock;
    private readonly CompressContextHandler _handler;
    private readonly Fixture _fixture;

    public CompressContextHandlerTests()
    {
        _strategyMock = new Mock<ICompressionStrategy>();
        _strategyMock.Setup(x => x.StrategyName).Returns("CodeStructure");
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var strategies = new List<ICompressionStrategy> { _strategyMock.Object };
        _handler = new CompressContextHandler(strategies);
    }

    [Fact(DisplayName = "Comprimir contexto com conteúdo válido retorna resultado comprimido")]
    [Trait("Layer", "Application - Commands")]
    public async Task ValidContent_Executed_ReturnsCompressedResult()
    {
        // Arrange
        var content = _fixture.Create<string>();
        var command = new CompressContextCommand(content);
        var expectedResult = new CompressionResult(
            CompressedContent: "compressed",
            OriginalContent: content,
            OriginalLength: content.Length,
            CompressedLength: 10,
            ActualRatio: 0.5,
            StrategyUsed: "CodeStructure");
        _strategyMock
            .Setup(x => x.CompressAsync(content, 0.7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("compressed", result.CompressedContent);
        Assert.Equal(content, result.OriginalContent);
        Assert.Equal(0.5, result.ActualRatio);
        Assert.Equal("CodeStructure", result.StrategyUsed);
        _strategyMock.Verify(x => x.CompressAsync(content, 0.7, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Comprimir contexto com conteúdo vazio lança ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmptyContent_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new CompressContextCommand("");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _strategyMock.Verify(
            x => x.CompressAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Comprimir contexto com whitespace lança ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task WhitespaceContent_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new CompressContextCommand("   ");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _strategyMock.Verify(
            x => x.CompressAsync(It.IsAny<string>(), It.IsAny<double>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact(DisplayName = "Comprimir contexto com targetRatio customizado usa ratio informado")]
    [Trait("Layer", "Application - Commands")]
    public async Task CustomTargetRatio_Executed_UsesProvidedRatio()
    {
        // Arrange
        var content = _fixture.Create<string>();
        var command = new CompressContextCommand(content, TargetRatio: 0.5);
        var expectedResult = new CompressionResult(
            CompressedContent: "compressed",
            OriginalContent: content,
            OriginalLength: content.Length,
            CompressedLength: 10,
            ActualRatio: 0.5,
            StrategyUsed: "CodeStructure");
        _strategyMock
            .Setup(x => x.CompressAsync(content, 0.5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _strategyMock.Verify(x => x.CompressAsync(content, 0.5, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Comprimir contexto com estratégia inexistente lança ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task UnknownStrategy_Executed_ThrowsArgumentException()
    {
        // Arrange
        var content = _fixture.Create<string>();
        var command = new CompressContextCommand(content, (CompressionStrategy)999);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Theory(DisplayName = "Comprimir contexto com targetRatio inválido lança ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    [InlineData(0.0)]
    [InlineData(-0.1)]
    [InlineData(1.0)]
    [InlineData(1.5)]
    public async Task InvalidTargetRatio_Executed_ThrowsArgumentException(double targetRatio)
    {
        // Arrange
        var content = _fixture.Create<string>();
        var command = new CompressContextCommand(content, TargetRatio: targetRatio);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
