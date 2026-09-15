using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Projects;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Enums;
using TaskManagement.DTO.Models.Project;

namespace TaskManagement.Command.Project.UpdateProjectStatus;

public sealed class UpdateProjectStatusCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateProjectStatusCommand> validator)
    : ICommandHandler<UpdateProjectStatusCommand, ProjectDetailsDto>
{
    public async Task<ProjectDetailsDto> HandleAsync(UpdateProjectStatusCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var project = await projectRepository.GetByIdAsync(command.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        var nextStatus = Enum.Parse<ProjectStatus>(command.Status.Trim(), true);

        if (!project.CanTransitionTo(nextStatus))
        {
            throw new ConflictException($"Project status cannot change from {project.Status} to {nextStatus}.");
        }

        project.Status = nextStatus;

        if (project.Status == ProjectStatus.Completed && !project.EndDateUtc.HasValue)
        {
            project.EndDateUtc = DateTime.UtcNow;
        }

        projectRepository.Update(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProject = await projectRepository.GetDetailsByIdAsync(project.Id, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        return updatedProject.ToDetailsDto();
    }
}