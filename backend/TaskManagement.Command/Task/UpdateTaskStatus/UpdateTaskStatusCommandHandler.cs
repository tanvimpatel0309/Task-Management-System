using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Task;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Command.Task.UpdateTaskStatus;

public sealed class UpdateTaskStatusCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateTaskStatusCommand> validator)
    : ICommandHandler<UpdateTaskStatusCommand, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> HandleAsync(UpdateTaskStatusCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var taskItem = await taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var updatedByUser = await userRepository.GetActiveByIdAsync(command.UpdatedByUserId, cancellationToken)
            ?? throw new NotFoundException("Active updater user was not found.");

        var nextStatus = Enum.Parse<DomainTaskStatus>(command.Status.Trim(), true);
        var previousStatus = taskItem.Status;

        taskItem.Status = nextStatus;

        if (nextStatus == DomainTaskStatus.Completed)
        {
            taskItem.ProgressPercentage = 100;
            taskItem.CompletedDateUtc = DateTime.UtcNow;
        }
        else if (taskItem.CompletedDateUtc.HasValue)
        {
            taskItem.CompletedDateUtc = null;
            if (taskItem.ProgressPercentage == 100)
            {
                taskItem.ProgressPercentage = 99;
            }
        }

        taskItemRepository.Update(taskItem);
        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "TaskStatusUpdated",
                Details = $"Task status changed from {previousStatus} to {nextStatus}.",
                OldValue = previousStatus.ToString(),
                NewValue = nextStatus.ToString(),
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