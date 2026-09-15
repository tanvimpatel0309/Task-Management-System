using TaskManagement.AppServices.Cqrs;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Projects;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.Project;

namespace TaskManagement.Queries.Project.GetProjectById;

public sealed class GetProjectByIdQueryHandler(IProjectRepository projectRepository)
    : IQueryHandler<GetProjectByIdQuery, ProjectDetailsDto>
{
    public async Task<ProjectDetailsDto> HandleAsync(GetProjectByIdQuery query, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetDetailsByIdAsync(query.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        return project.ToDetailsDto();
    }
}