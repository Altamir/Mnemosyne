namespace Mnemosyne.Domain.Entities;

public class ProjectIndexJobEntity
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public IndexStatus Status { get; private set; }
    public int TotalMemories { get; private set; }
    public int ProcessedMemories { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }

    private ProjectIndexJobEntity() { }

    public static ProjectIndexJobEntity Create(Guid projectId)
    {
        if (projectId == Guid.Empty)
        {
            throw new ArgumentException("ProjectId cannot be empty", nameof(projectId));
        }

        return new ProjectIndexJobEntity
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Status = IndexStatus.Pending,
            TotalMemories = 0,
            ProcessedMemories = 0,
            ErrorMessage = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkAsProcessing()
    {
        if (Status == IndexStatus.Completed || Status == IndexStatus.Failed)
        {
            throw new InvalidOperationException("Cannot mark a completed or failed job as processing");
        }

        Status = IndexStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted(int processedCount)
    {
        if (processedCount < 0)
        {
            throw new ArgumentException("Processed count cannot be negative", nameof(processedCount));
        }

        Status = IndexStatus.Completed;
        ProcessedMemories = processedCount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));
        }

        Status = IndexStatus.Failed;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTotalMemories(int total)
    {
        if (total < 0)
        {
            throw new ArgumentException("Total memories cannot be negative", nameof(total));
        }

        TotalMemories = total;
        UpdatedAt = DateTime.UtcNow;
    }
}
