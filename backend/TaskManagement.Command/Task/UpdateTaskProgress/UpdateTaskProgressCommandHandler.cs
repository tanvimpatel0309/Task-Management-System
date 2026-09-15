using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Task;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Command.Task.UpdateTaskProgress;

public sealed class UpdateTaskProgressCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateTaskProgressCommand> validator)
    : ICommandHandler<UpdateTaskProgressCommand, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> HandleAsync(UpdateTaskProgressCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var taskItem = await taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var updatedByUser = await userRepository.GetActiveByIdAsync(command.UpdatedByUserId, cancellationToken)
            ?? throw new NotFoundException("Active updater user was not found.");

        var previousProgress = taskItem.ProgressPercentage;
        taskItem.ProgressPercentage = command.ProgressPercentage;

        if (command.ProgressPercentage >= 100)
        {
            taskItem.ProgressPercentage = 100;
            taskItem.Status = DomainTaskStatus.Completed;
            taskItem.CompletedDateUtc = DateTime.UtcNow;
        }
        else if (taskItem.Status == DomainTaskStatus.Completed)
        {
            taskItem.Status = DomainTaskStatus.InProgress;
            taskItem.CompletedDateUtc = null;
        }

        taskItemRepository.Update(taskItem);
        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "TaskProgressUpdated",
                Details = $"Task progress changed from {previousProgress}% to {taskItem.ProgressPercentage}%.",
                OldValue = previousProgress.ToString(),
                NewValue = taskItem.ProgressPercentage.ToString(),
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