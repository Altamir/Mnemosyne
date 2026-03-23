using AutoFixture;
using Mnemosyne.Application.Features.Project.UpdateProject;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Project;

public class UpdateProjectHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly UpdateProjectHandler _handler;
    private readonly Fixture _fixture;

    public UpdateProjectHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new UpdateProjectHandler(_repositoryMock.Object);
    }

    [Fact(DisplayName = "Atualizar projeto existente com nome valido retorna projeto atualizado")]
    [Trait("Layer", "Application - Commands")]
    public async Task ValidUpdate_Executed_ReturnsUpdatedProject()
    {
        // Arrange
        var project = ProjectEntity.Create(_fixture.Create<string>(), Guid.NewGuid());
        var newName = _fixture.Create<string>();
        var newDescription = _fixture.Create<string>();
        _repositoryMock
            .Setup(x => x.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectEntity p, CancellationToken _) => p);
        var command = new UpdateProjectCommand(project.Id, newName, newDescription);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);
        Assert.Equal(newDescription, result.Description);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Atualizar projeto inexistente lanca KeyNotFoundException")]
    [Trait("Layer", "Application - Commands")]
    public async Task NonExistingProject_Executed_ThrowsKeyNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _repositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectEntity?)null);
        var command = new UpdateProjectCommand(projectId, _fixture.Create<string>());

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Atualizar projeto com nome vazio lanca ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmptyName_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new UpdateProjectCommand(Guid.NewGuid(), string.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Atualizar projeto com id vazio lanca ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmptyId_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new UpdateProjectCommand(Guid.Empty, _fixture.Create<string>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
