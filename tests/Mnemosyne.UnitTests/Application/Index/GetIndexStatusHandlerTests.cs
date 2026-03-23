using AutoFixture;
using Mnemosyne.Application.Features.Index.GetIndexStatus;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Index;

public class GetIndexStatusHandlerTests
{
    private readonly Mock<IProjectIndexJobRepository> _repositoryMock;
    private readonly GetIndexStatusHandler _handler;
    private readonly Fixture _fixture;

    public GetIndexStatusHandlerTests()
    {
        _repositoryMock = new Mock<IProjectIndexJobRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new GetIndexStatusHandler(_repositoryMock.Object);
    }

    [Fact(DisplayName = "Job encontrado retorna status correto")]
    [Trait("Layer", "Application - Queries")]
    public async Task JobFound_Executed_ReturnsStatus()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var job = ProjectIndexJobEntity.Create(projectId);
        job.MarkAsProcessing();
        var query = new GetIndexStatusQuery(projectId);

        _repositoryMock
            .Setup(x => x.GetLatestByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(IndexStatus.Processing, result.Status);
        _repositoryMock.Verify(x => x.GetLatestByProjectIdAsync(projectId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Job nao encontrado retorna null")]
    [Trait("Layer", "Application - Queries")]
    public async Task JobNotFound_Executed_ReturnsNull()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var query = new GetIndexStatusQuery(projectId);

        _repositoryMock
            .Setup(x => x.GetLatestByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectIndexJobEntity?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "Job completado retorna status Completed")]
    [Trait("Layer", "Application - Queries")]
    public async Task CompletedJob_Executed_ReturnsCompletedStatus()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var job = ProjectIndexJobEntity.Create(projectId);
        job.MarkAsCompleted(10);
        var query = new GetIndexStatusQuery(projectId);

        _repositoryMock
            .Setup(x => x.GetLatestByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(IndexStatus.Completed, result.Status);
        Assert.Equal(10, result.ProcessedMemories);
    }

    [Fact(DisplayName = "Job falhou retorna status Failed")]
    [Trait("Layer", "Application - Queries")]
    public async Task FailedJob_Executed_ReturnsFailedStatus()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var job = ProjectIndexJobEntity.Create(projectId);
        job.MarkAsFailed("Connection timeout");
        var query = new GetIndexStatusQuery(projectId);

        _repositoryMock
            .Setup(x => x.GetLatestByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(job);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(IndexStatus.Failed, result.Status);
        Assert.Equal("Connection timeout", result.ErrorMessage);
    }
}
