using TaskManagement.AppServices.Cqrs;
using TaskManagement.AppServices.Projects;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.Project;

namespace TaskManagement.Queries.Project.GetProjects;

public sealed class GetProjectsQueryHandler(IProjectRepository projectRepository)
    : IQueryHandler<GetProjectsQuery, IReadOnlyList<ProjectListItemDto>>
{
    public async Task<IReadOnlyList<ProjectListItemDto>> HandleAsync(GetProjectsQuery query, CancellationToken cancellationToken = default)
    {
        var projects = await projectRepository.ListProjectsAsync(cancellationToken);
        return projects.Select(project => project.ToListItemDto()).ToList();
    }
}