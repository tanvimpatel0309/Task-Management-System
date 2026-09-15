using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Projects;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.Project;

namespace TaskManagement.Command.Project.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IProjectRepository projectRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateProjectCommand> validator)
    : ICommandHandler<UpdateProjectCommand, ProjectDetailsDto>
{
    public async Task<ProjectDetailsDto> HandleAsync(UpdateProjectCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var project = await projectRepository.GetByIdAsync(command.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        project.Name = command.Name.Trim();
        project.Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim();
        project.StartDateUtc = command.StartDateUtc;
        project.EndDateUtc = command.EndDateUtc;

        projectRepository.Update(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProject = await projectRepository.GetDetailsByIdAsync(project.Id, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        return updatedProject.ToDetailsDto();
    }
}