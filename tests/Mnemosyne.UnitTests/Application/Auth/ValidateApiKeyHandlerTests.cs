using AutoFixture;
using Mnemosyne.Application.Features.Auth.ValidateApiKey;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Auth;

public class ValidateApiKeyHandlerTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly ValidateApiKeyHandler _handler;
    private readonly Fixture _fixture;

    public ValidateApiKeyHandlerTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new ValidateApiKeyHandler(_repositoryMock.Object);
    }

    [Fact(DisplayName = "API Key valida retorna usuario correspondente")]
    [Trait("Layer", "Application - Queries")]
    public async Task ValidApiKey_Executed_ReturnsUser()
    {
        // Arrange
        var apiKey = _fixture.Create<string>();
        var user = UserEntity.Create(apiKey, "testuser@test.com");
        var query = new ValidateApiKeyQuery(apiKey);
        _repositoryMock
            .Setup(x => x.GetByApiKeyHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        _repositoryMock.Verify(x => x.GetByApiKeyHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "API Key invalida retorna null")]
    [Trait("Layer", "Application - Queries")]
    public async Task InvalidApiKey_Executed_ReturnsNull()
    {
        // Arrange
        var apiKey = _fixture.Create<string>();
        var query = new ValidateApiKeyQuery(apiKey);
        _repositoryMock
            .Setup(x => x.GetByApiKeyHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(x => x.GetByApiKeyHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "API Key vazia lanca ArgumentException")]
    [Trait("Layer", "Application - Queries")]
    public async Task EmptyApiKey_Executed_ThrowsArgumentException()
    {
        // Arrange
        var query = new ValidateApiKeyQuery(string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
        _repositoryMock.Verify(x => x.GetByApiKeyHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "API Key whitespace lanca ArgumentException")]
    [Trait("Layer", "Application - Queries")]
    public async Task WhitespaceApiKey_Executed_ThrowsArgumentException()
    {
        // Arrange
        var query = new ValidateApiKeyQuery("   ");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
        _repositoryMock.Verify(x => x.GetByApiKeyHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}