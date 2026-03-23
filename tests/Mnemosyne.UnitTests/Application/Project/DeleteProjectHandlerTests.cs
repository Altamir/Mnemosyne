using AutoFixture;
using Mnemosyne.Application.Features.Project.DeleteProject;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Project;

public class DeleteProjectHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly DeleteProjectHandler _handler;
    private readonly Fixture _fixture;

    public DeleteProjectHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new DeleteProjectHandler(_repositoryMock.Object);
    }

    [Fact(DisplayName = "Deletar projeto existente executa com sucesso")]
    [Trait("Layer", "Application - Commands")]
    public async Task ExistingProject_Executed_DeletesSuccessfully()
    {
        // Arrange
        var project = ProjectEntity.Create(_fixture.Create<string>(), Guid.NewGuid());
        _repositoryMock
            .Setup(x => x.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _repositoryMock
            .Setup(x => x.DeleteAsync(project, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var command = new DeleteProjectCommand(project.Id);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.GetByIdAsync(project.Id, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(x => x.DeleteAsync(project, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deletar projeto inexistente lanca KeyNotFoundException")]
    [Trait("Layer", "Application - Commands")]
    public async Task NonExistingProject_Executed_ThrowsKeyNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        _repositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectEntity?)null);
        var command = new DeleteProjectCommand(projectId);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.DeleteAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deletar projeto com id vazio lanca ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmptyId_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
