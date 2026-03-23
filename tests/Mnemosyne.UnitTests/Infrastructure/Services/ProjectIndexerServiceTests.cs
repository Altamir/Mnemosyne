using Microsoft.Extensions.Logging;
using Moq;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Mnemosyne.Infrastructure.Services;

namespace Mnemosyne.UnitTests.Infrastructure.Services;

public class ProjectIndexerServiceTests
{
    private readonly Mock<IProjectIndexJobRepository> _indexJobRepositoryMock;
    private readonly Mock<IEmbeddingService> _embeddingServiceMock;
    private readonly Mock<IMemoryRepository> _memoryRepositoryMock;
    private readonly Mock<ILogger<ProjectIndexerService>> _loggerMock;

    public ProjectIndexerServiceTests()
    {
        _indexJobRepositoryMock = new Mock<IProjectIndexJobRepository>();
        _embeddingServiceMock = new Mock<IEmbeddingService>();
        _memoryRepositoryMock = new Mock<IMemoryRepository>();
        _loggerMock = new Mock<ILogger<ProjectIndexerService>>();
    }

    [Fact(DisplayName = "ProcessPendingJobs - Job pendente e processado com sucesso")]
    [Trait("Layer", "Infrastructure - Services")]
    public async Task PendingJob_ProcessPendingJobs_MarksAsCompleted()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var job = ProjectIndexJobEntity.Create(projectId);

        _indexJobRepositoryMock
            .Setup(x => x.GetPendingJobsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectIndexJobEntity> { job });
        _indexJobRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectIndexJobEntity j, CancellationToken _) => j);

        var service = CreateService();

        // Act
        await service.ProcessPendingJobsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(IndexStatus.Completed, job.Status);
        _indexJobRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact(DisplayName = "ProcessPendingJobs - Sem jobs pendentes nao faz nada")]
    [Trait("Layer", "Infrastructure - Services")]
    public async Task NoJobs_ProcessPendingJobs_DoesNothing()
    {
        // Arrange
        _indexJobRepositoryMock
            .Setup(x => x.GetPendingJobsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectIndexJobEntity>());

        var service = CreateService();

        // Act
        await service.ProcessPendingJobsAsync(CancellationToken.None);

        // Assert
        _indexJobRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "ProcessPendingJobs - Erro durante processamento marca job como Failed")]
    [Trait("Layer", "Infrastructure - Services")]
    public async Task ErrorDuringProcessing_ProcessPendingJobs_MarksAsFailed()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var job = ProjectIndexJobEntity.Create(projectId);

        _indexJobRepositoryMock
            .Setup(x => x.GetPendingJobsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectIndexJobEntity> { job });

        // First call succeeds (mark as processing), second throws (simulate error during indexing)
        var callCount = 0;
        _indexJobRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()))
            .Returns((ProjectIndexJobEntity j, CancellationToken _) =>
            {
                callCount++;
                if (callCount == 2) // Second update (after MarkAsProcessing) simulates indexing error
                    throw new Exception("Database connection lost");
                return Task.FromResult(j);
            });

        var service = CreateService();

        // Act
        await service.ProcessPendingJobsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(IndexStatus.Failed, job.Status);
        Assert.Contains("Database connection lost", job.ErrorMessage);
    }

    [Fact(DisplayName = "ProcessPendingJobs - Job transiciona para Processing antes de completar")]
    [Trait("Layer", "Infrastructure - Services")]
    public async Task PendingJob_ProcessPendingJobs_TransitionsToProcessingFirst()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var job = ProjectIndexJobEntity.Create(projectId);
        var statusesObserved = new List<IndexStatus>();

        _indexJobRepositoryMock
            .Setup(x => x.GetPendingJobsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectIndexJobEntity> { job });
        _indexJobRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()))
            .Callback<ProjectIndexJobEntity, CancellationToken>((j, _) => statusesObserved.Add(j.Status))
            .ReturnsAsync((ProjectIndexJobEntity j, CancellationToken _) => j);

        var service = CreateService();

        // Act
        await service.ProcessPendingJobsAsync(CancellationToken.None);

        // Assert
        Assert.True(statusesObserved.Count >= 2);
        Assert.Equal(IndexStatus.Processing, statusesObserved[0]);
        Assert.Equal(IndexStatus.Completed, statusesObserved[^1]);
    }

    [Fact(DisplayName = "ProcessPendingJobs - Multiplos jobs pendentes sao processados")]
    [Trait("Layer", "Infrastructure - Services")]
    public async Task MultipleJobs_ProcessPendingJobs_ProcessesAll()
    {
        // Arrange
        var job1 = ProjectIndexJobEntity.Create(Guid.NewGuid());
        var job2 = ProjectIndexJobEntity.Create(Guid.NewGuid());

        _indexJobRepositoryMock
            .Setup(x => x.GetPendingJobsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectIndexJobEntity> { job1, job2 });
        _indexJobRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<ProjectIndexJobEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectIndexJobEntity j, CancellationToken _) => j);

        var service = CreateService();

        // Act
        await service.ProcessPendingJobsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(IndexStatus.Completed, job1.Status);
        Assert.Equal(IndexStatus.Completed, job2.Status);
    }

    private ProjectIndexerService CreateService()
    {
        return new ProjectIndexerService(
            _indexJobRepositoryMock.Object,
            _embeddingServiceMock.Object,
            _memoryRepositoryMock.Object,
            _loggerMock.Object);
    }
}
