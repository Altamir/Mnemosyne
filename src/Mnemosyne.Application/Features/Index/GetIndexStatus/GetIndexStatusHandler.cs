using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Index.GetIndexStatus;

public class GetIndexStatusHandler
{
    private readonly IProjectIndexJobRepository _indexJobRepository;

    public GetIndexStatusHandler(IProjectIndexJobRepository indexJobRepository)
    {
        _indexJobRepository = indexJobRepository;
    }

    public async Task<GetIndexStatusResult?> Handle(GetIndexStatusQuery query, CancellationToken cancellationToken)
    {
        var job = await _indexJobRepository.GetLatestByProjectIdAsync(query.ProjectId, cancellationToken);
        if (job == null)
        {
            return null;
        }

        return new GetIndexStatusResult(
            job.ProjectId,
            job.Status,
            job.TotalMemories,
            job.ProcessedMemories,
            job.ErrorMessage,
            job.CreatedAt,
            job.UpdatedAt
        );
    }
}
