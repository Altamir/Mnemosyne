using AutoFixture;
using Mnemosyne.Application.Features.Index.StartProjectIndex;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Index;

public class StartProjectIndexHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IProjectIndexJobRepository> _indexJobRepositoryMock;
    private readonly StartProjectIndexHandler _handler;
    private readonly Fixture _fixture;

    public StartProjectIndexHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _indexJobRepositoryMock = new Mock<IProjectIndexJobRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new StartProjectIndexHandler(_projectRepositoryMock.Object, _indexJobRepositoryMock.Object);
    }

    [Fact(DisplayName = "Projeto existente inicia indexacao e retorna job")]
    [Trait("Layer", "Application - Commands")]
    public async Task ExistingProject_Executed_ReturnsCreatedJob()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectEntity.Create(_fixture.Create<string>(), userId);
        var command = new StartProjectIndexCommand(projectId, userId);
        var createdJob = ProjectIndexJobEntity.Create(projectId);

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _indexJobRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdJob);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(IndexStatus.Pending, result.Status);
        _projectRepositoryMock.Verify(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
        _indexJobRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Projeto nao encontrado lanca KeyNotFoundException")]
    [Trait("Layer", "Application - Commands")]
    public async Task ProjectNotFound_Executed_ThrowsKeyNotFoundException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new StartProjectIndexCommand(projectId, userId);

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _projectRepositoryMock.Verify(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
        _indexJobRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Indexacao ja em andamento para mesmo projeto lanca InvalidOperationException")]
    [Trait("Layer", "Application - Commands")]
    public async Task IndexAlreadyRunning_Executed_ThrowsInvalidOperationException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectEntity.Create(_fixture.Create<string>(), userId);
        var existingJob = ProjectIndexJobEntity.Create(projectId);
        existingJob.MarkAsProcessing();
        var command = new StartProjectIndexCommand(projectId, userId);

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _indexJobRepositoryMock
            .Setup(x => x.GetPendingOrProcessingByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingJob);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "UserId invalido lanca ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task InvalidUserId_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new StartProjectIndexCommand(Guid.NewGuid(), Guid.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Projeto sem memorias inicia indexacao com zero memories")]
    [Trait("Layer", "Application - Commands")]
    public async Task ProjectWithoutMemories_Executed_ReturnsJobWithZeroMemoryCount()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = ProjectEntity.Create(_fixture.Create<string>(), userId);
        var command = new StartProjectIndexCommand(projectId, userId);
        var createdJob = ProjectIndexJobEntity.Create(projectId);

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);
        _indexJobRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdJob);
        _indexJobRepositoryMock
            .Setup(x => x.GetPendingOrProcessingByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectIndexJobEntity?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalMemories);
    }
}
