using AutoFixture;
using Mnemosyne.Application.Features.Project.ListProjects;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Project;

public class ListProjectsHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly ListProjectsHandler _handler;
    private readonly Fixture _fixture;

    public ListProjectsHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new ListProjectsHandler(_repositoryMock.Object);
    }

    [Fact(DisplayName = "Listar projetos de usuario com projetos retorna lista")]
    [Trait("Layer", "Application - Queries")]
    public async Task UserWithProjects_Executed_ReturnsList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projects = new List<ProjectEntity>
        {
            ProjectEntity.Create(_fixture.Create<string>(), userId),
            ProjectEntity.Create(_fixture.Create<string>(), userId)
        };
        _repositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects.AsReadOnly());
        var query = new ListProjectsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _repositoryMock.Verify(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Listar projetos de usuario sem projetos retorna lista vazia")]
    [Trait("Layer", "Application - Queries")]
    public async Task UserWithoutProjects_Executed_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _repositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectEntity>().AsReadOnly());
        var query = new ListProjectsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "Listar projetos com userId vazio lanca ArgumentException")]
    [Trait("Layer", "Application - Queries")]
    public async Task EmptyUserId_Executed_ThrowsArgumentException()
    {
        // Arrange
        var query = new ListProjectsQuery(Guid.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
        _repositoryMock.Verify(x => x.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
