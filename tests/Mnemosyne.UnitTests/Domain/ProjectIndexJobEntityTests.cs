using Mnemosyne.Domain.Entities;

namespace Mnemosyne.UnitTests.Domain;

public class ProjectIndexJobEntityTests
{
    #region Create

    [Fact(DisplayName = "Create - ProjectId valido retorna job com status Pending")]
    [Trait("Layer", "Domain - Entities")]
    public void ValidProjectId_Create_ReturnsJobWithPendingStatus()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        // Act
        var job = ProjectIndexJobEntity.Create(projectId);

        // Assert
        Assert.NotEqual(Guid.Empty, job.Id);
        Assert.Equal(projectId, job.ProjectId);
        Assert.Equal(IndexStatus.Pending, job.Status);
        Assert.Equal(0, job.TotalMemories);
        Assert.Equal(0, job.ProcessedMemories);
        Assert.Null(job.ErrorMessage);
        Assert.True(job.CreatedAt <= DateTime.UtcNow);
        Assert.True(job.UpdatedAt <= DateTime.UtcNow);
    }

    [Fact(DisplayName = "Create - ProjectId vazio lanca ArgumentException")]
    [Trait("Layer", "Domain - Entities")]
    public void EmptyProjectId_Create_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => ProjectIndexJobEntity.Create(Guid.Empty));
        Assert.Contains("ProjectId", exception.Message);
    }

    #endregion

    #region MarkAsProcessing

    [Fact(DisplayName = "MarkAsProcessing - Job Pending transiciona para Processing")]
    [Trait("Layer", "Domain - Entities")]
    public void PendingJob_MarkAsProcessing_TransitionsToProcessing()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());

        // Act
        job.MarkAsProcessing();

        // Assert
        Assert.Equal(IndexStatus.Processing, job.Status);
    }

    [Fact(DisplayName = "MarkAsProcessing - Job Completed lanca InvalidOperationException")]
    [Trait("Layer", "Domain - Entities")]
    public void CompletedJob_MarkAsProcessing_ThrowsInvalidOperationException()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());
        job.MarkAsCompleted(5);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => job.MarkAsProcessing());
    }

    [Fact(DisplayName = "MarkAsProcessing - Job Failed lanca InvalidOperationException")]
    [Trait("Layer", "Domain - Entities")]
    public void FailedJob_MarkAsProcessing_ThrowsInvalidOperationException()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());
        job.MarkAsFailed("Some error");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => job.MarkAsProcessing());
    }

    #endregion

    #region MarkAsCompleted

    [Fact(DisplayName = "MarkAsCompleted - Contagem valida define status e ProcessedMemories")]
    [Trait("Layer", "Domain - Entities")]
    public void ValidCount_MarkAsCompleted_SetsStatusAndProcessedMemories()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());
        job.MarkAsProcessing();

        // Act
        job.MarkAsCompleted(10);

        // Assert
        Assert.Equal(IndexStatus.Completed, job.Status);
        Assert.Equal(10, job.ProcessedMemories);
    }

    [Fact(DisplayName = "MarkAsCompleted - Contagem negativa lanca ArgumentException")]
    [Trait("Layer", "Domain - Entities")]
    public void NegativeCount_MarkAsCompleted_ThrowsArgumentException()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());
        job.MarkAsProcessing();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => job.MarkAsCompleted(-1));
    }

    #endregion

    #region MarkAsFailed

    [Fact(DisplayName = "MarkAsFailed - Mensagem valida define status e ErrorMessage")]
    [Trait("Layer", "Domain - Entities")]
    public void ValidMessage_MarkAsFailed_SetsStatusAndErrorMessage()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());
        job.MarkAsProcessing();

        // Act
        job.MarkAsFailed("Connection timeout");

        // Assert
        Assert.Equal(IndexStatus.Failed, job.Status);
        Assert.Equal("Connection timeout", job.ErrorMessage);
    }

    [Fact(DisplayName = "MarkAsFailed - Mensagem vazia lanca ArgumentException")]
    [Trait("Layer", "Domain - Entities")]
    public void EmptyMessage_MarkAsFailed_ThrowsArgumentException()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => job.MarkAsFailed(""));
    }

    [Fact(DisplayName = "MarkAsFailed - Mensagem whitespace lanca ArgumentException")]
    [Trait("Layer", "Domain - Entities")]
    public void WhitespaceMessage_MarkAsFailed_ThrowsArgumentException()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => job.MarkAsFailed("   "));
    }

    #endregion

    #region SetTotalMemories

    [Fact(DisplayName = "SetTotalMemories - Valor valido define TotalMemories")]
    [Trait("Layer", "Domain - Entities")]
    public void ValidTotal_SetTotalMemories_SetsTotalMemories()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());

        // Act
        job.SetTotalMemories(100);

        // Assert
        Assert.Equal(100, job.TotalMemories);
    }

    [Fact(DisplayName = "SetTotalMemories - Zero e valor valido")]
    [Trait("Layer", "Domain - Entities")]
    public void ZeroTotal_SetTotalMemories_SetsZero()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());

        // Act
        job.SetTotalMemories(0);

        // Assert
        Assert.Equal(0, job.TotalMemories);
    }

    [Fact(DisplayName = "SetTotalMemories - Valor negativo lanca ArgumentException")]
    [Trait("Layer", "Domain - Entities")]
    public void NegativeTotal_SetTotalMemories_ThrowsArgumentException()
    {
        // Arrange
        var job = ProjectIndexJobEntity.Create(Guid.NewGuid());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => job.SetTotalMemories(-1));
    }

    #endregion
}
