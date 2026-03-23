using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;

namespace Mnemosyne.Application.Features.Project.ListProjects;

public class ListProjectsHandler
{
    private readonly IProjectRepository _projectRepository;

    public ListProjectsHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IReadOnlyList<ProjectEntity>> Handle(ListProjectsQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty", nameof(query.UserId));
        }

        return await _projectRepository.GetByUserIdAsync(query.UserId, cancellationToken);
    }
}
