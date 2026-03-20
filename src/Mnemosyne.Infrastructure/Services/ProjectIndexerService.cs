using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Infrastructure.Services;

public class ProjectIndexerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProjectIndexerService> _logger;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(10);

    public ProjectIndexerService(
        IServiceScopeFactory scopeFactory,
        ILogger<ProjectIndexerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    /// <summary>
    /// Constructor for unit testing — accepts dependencies directly instead of scope factory.
    /// </summary>
    internal ProjectIndexerService(
        IProjectIndexJobRepository indexJobRepository,
        IEmbeddingService embeddingService,
        IMemoryRepository memoryRepository,
        ILogger<ProjectIndexerService> logger)
    {
        _scopeFactory = null!;
        _logger = logger;
        _testIndexJobRepository = indexJobRepository;
        _testEmbeddingService = embeddingService;
        _testMemoryRepository = memoryRepository;
    }

    // Fields for test-only constructor
    private readonly IProjectIndexJobRepository? _testIndexJobRepository;
    private readonly IEmbeddingService? _testEmbeddingService;
    private readonly IMemoryRepository? _testMemoryRepository;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ProjectIndexerService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingJobsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ProjectIndexerService polling loop");
            }

            await Task.Delay(PollingInterval, stoppingToken);
        }

        _logger.LogInformation("ProjectIndexerService stopped");
    }

    public async Task ProcessPendingJobsAsync(CancellationToken cancellationToken)
    {
        IProjectIndexJobRepository indexJobRepository;

        if (_testIndexJobRepository is not null)
        {
            indexJobRepository = _testIndexJobRepository;
        }
        else
        {
            using var scope = _scopeFactory.CreateScope();
            indexJobRepository = scope.ServiceProvider.GetRequiredService<IProjectIndexJobRepository>();
            await ProcessJobsWithRepository(indexJobRepository, cancellationToken);
            return;
        }

        await ProcessJobsWithRepository(indexJobRepository, cancellationToken);
    }

    private async Task ProcessJobsWithRepository(IProjectIndexJobRepository indexJobRepository, CancellationToken cancellationToken)
    {
        var pendingJobs = await indexJobRepository.GetPendingJobsAsync(cancellationToken);

        foreach (var job in pendingJobs)
        {
            await ProcessSingleJobAsync(job, indexJobRepository, cancellationToken);
        }
    }

    private async Task ProcessSingleJobAsync(
        ProjectIndexJobEntity job,
        IProjectIndexJobRepository indexJobRepository,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing index job {JobId} for project {ProjectId}", job.Id, job.ProjectId);

            job.MarkAsProcessing();
            await indexJobRepository.UpdateAsync(job, cancellationToken);

            // Currently, MemoryEntity has no ProjectId field, so we cannot
            // filter memories by project. The indexing marks the job as completed
            // with 0 processed memories. Real embedding generation will be
            // added when the memory-project relationship is established.
            job.MarkAsCompleted(0);
            await indexJobRepository.UpdateAsync(job, cancellationToken);

            _logger.LogInformation("Completed index job {JobId} for project {ProjectId}", job.Id, job.ProjectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process index job {JobId} for project {ProjectId}", job.Id, job.ProjectId);

            try
            {
                job.MarkAsFailed(ex.Message);
                await indexJobRepository.UpdateAsync(job, cancellationToken);
            }
            catch (Exception innerEx)
            {
                _logger.LogError(innerEx, "Failed to mark job {JobId} as failed", job.Id);
            }
        }
    }
}
