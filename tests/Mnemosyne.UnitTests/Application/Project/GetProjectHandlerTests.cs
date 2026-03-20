using AutoFixture;
using Mnemosyne.Application.Features.Project.GetProject;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Project;

public class GetProjectHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly GetProjectHandler _handler;
    private readonly Fixture _fixture;

    public GetProjectHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new GetProjectHandler(_repositoryMock.Object);
    }

    [Fact(DisplayName = "Buscar projeto existente retorna projeto")]
    [Trait("Layer", "Application - Queries")]
    public async Task ExistingProject_Executed_ReturnsProject()
    {
        // Arrange
        var project = ProjectEntity.Create(_fixture.Create<string>(), Guid.NewGuid());
        _repositoryMock
            .Setup(x => x.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        var query = new GetProjectQuery(project.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(project.Id, result.Id);
        Assert.Equal(project.Name, result.Name);
        _repositoryMock.Verify(x => x.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Buscar projeto inexistente lanca KeyNotFoundException")]
    [Trait("Layer", "Application - Queries")]
    public async Task NonExistingProject_Executed_ThrowsKeyNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _repositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectEntity?)null);
        var query = new GetProjectQuery(projectId);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact(DisplayName = "Buscar projeto com id vazio lanca ArgumentException")]
    [Trait("Layer", "Application - Queries")]
    public async Task EmptyId_Executed_ThrowsArgumentException()
    {
        // Arrange
        var query = new GetProjectQuery(Guid.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
