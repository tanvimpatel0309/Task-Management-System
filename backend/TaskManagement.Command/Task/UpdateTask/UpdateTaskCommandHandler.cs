using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Task;

namespace TaskManagement.Command.Task.UpdateTask;

public sealed class UpdateTaskCommandHandler(
    ITaskItemRepository taskItemRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateTaskCommand> validator)
    : ICommandHandler<UpdateTaskCommand, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> HandleAsync(UpdateTaskCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var taskItem = await taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var project = await projectRepository.GetByIdAsync(command.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        if (!project.CanAcceptTasks())
        {
            throw new ConflictException("Archived projects cannot receive tasks.");
        }

        var updatedByUser = await userRepository.GetActiveByIdAsync(command.UpdatedByUserId, cancellationToken)
            ?? throw new NotFoundException("Active updater user was not found.");

        var previousState = $"Title={taskItem.Title}; ProjectId={taskItem.ProjectId}; DueDateUtc={taskItem.DueDateUtc:o}";

        taskItem.Title = command.Title.Trim();
        taskItem.Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim();
        taskItem.StartDateUtc = command.StartDateUtc;
        taskItem.DueDateUtc = command.DueDateUtc;
        taskItem.ProjectId = project.Id;

        taskItemRepository.Update(taskItem);
        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "TaskUpdated",
                Details = $"Task '{taskItem.Title}' details were updated.",
                OldValue = previousState,
                NewValue = $"Title={taskItem.Title}; ProjectId={taskItem.ProjectId}; DueDateUtc={taskItem.DueDateUtc:o}",
                UserId = updatedByUser.Id,
                ProjectId = taskItem.ProjectId,
                TaskItemId = taskItem.Id
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedTask = await taskItemRepository.GetByIdWithDetailsAsync(taskItem.Id, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        return updatedTask.ToDetailsDto();
    }
}