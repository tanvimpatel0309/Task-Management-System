using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Projects;
using TaskManagement.Domain.Contracts;
using DomainProject = TaskManagement.Domain.Entities.Project;
using TaskManagement.Domain.Enums;
using TaskManagement.DTO.Models.Project;

namespace TaskManagement.Command.Project.CreateProject;

public sealed class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateProjectCommand> validator)
    : ICommandHandler<CreateProjectCommand, ProjectDetailsDto>
{
    public async Task<ProjectDetailsDto> HandleAsync(CreateProjectCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var createdByUser = await userRepository.GetActiveByIdAsync(command.CreatedByUserId, cancellationToken)
            ?? throw new NotFoundException("Active creator user was not found.");

        var status = string.IsNullOrWhiteSpace(command.Status)
            ? ProjectStatus.Planning
            : Enum.Parse<ProjectStatus>(command.Status.Trim(), true);

        var project = new DomainProject
        {
            Id = Guid.NewGuid(),
            Name = command.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim(),
            Status = status,
            StartDateUtc = command.StartDateUtc,
            EndDateUtc = command.EndDateUtc,
            CreatedByUserId = createdByUser.Id
        };

        await projectRepository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return project.ToDetailsDto();
    }
}