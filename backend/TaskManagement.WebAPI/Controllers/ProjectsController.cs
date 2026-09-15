using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Cqrs;
using TaskManagement.Command.Project.CreateProject;
using TaskManagement.Command.Project.DeleteProject;
using TaskManagement.Command.Project.UpdateProject;
using TaskManagement.Command.Project.UpdateProjectStatus;
using TaskManagement.DTO.Models.Project;
using TaskManagement.Queries.Project.GetProjectById;
using TaskManagement.Queries.Project.GetProjects;

namespace TaskManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
public sealed class ProjectsController(
    ICommandHandler<CreateProjectCommand, ProjectDetailsDto> createProjectCommandHandler,
    ICommandHandler<UpdateProjectCommand, ProjectDetailsDto> updateProjectCommandHandler,
    ICommandHandler<DeleteProjectCommand, bool> deleteProjectCommandHandler,
    ICommandHandler<UpdateProjectStatusCommand, ProjectDetailsDto> updateProjectStatusCommandHandler,
    IQueryHandler<GetProjectsQuery, IReadOnlyList<ProjectListItemDto>> getProjectsQueryHandler,
    IQueryHandler<GetProjectByIdQuery, ProjectDetailsDto> getProjectByIdQueryHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProjectListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ProjectListItemDto>>> GetProjects(CancellationToken cancellationToken)
    {
        var response = await getProjectsQueryHandler.HandleAsync(new GetProjectsQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailsDto>> GetProjectById(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await getProjectByIdQueryHandler.HandleAsync(new GetProjectByIdQuery { ProjectId = projectId }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailsDto>> CreateProject(
        [FromBody] CreateProjectRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await createProjectCommandHandler.HandleAsync(
            new CreateProjectCommand
            {
                Name = request.Name,
                Description = request.Description,
                StartDateUtc = request.StartDateUtc,
                EndDateUtc = request.EndDateUtc,
                Status = request.Status,
                CreatedByUserId = GetCurrentUserId()
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetProjectById), new { projectId = response.Id }, response);
    }

    [HttpPut("{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailsDto>> UpdateProject(
        Guid projectId,
        [FromBody] UpdateProjectRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateProjectCommandHandler.HandleAsync(
            new UpdateProjectCommand
            {
                ProjectId = projectId,
                Name = request.Name,
                Description = request.Description,
                StartDateUtc = request.StartDateUtc,
                EndDateUtc = request.EndDateUtc
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{projectId:guid}/status")]
    [ProducesResponseType(typeof(ProjectDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDetailsDto>> UpdateProjectStatus(
        Guid projectId,
        [FromBody] UpdateProjectStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateProjectStatusCommandHandler.HandleAsync(
            new UpdateProjectStatusCommand
            {
                ProjectId = projectId,
                Status = request.Status
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(Guid projectId, CancellationToken cancellationToken)
    {
        await deleteProjectCommandHandler.HandleAsync(new DeleteProjectCommand { ProjectId = projectId }, cancellationToken);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
    }
}