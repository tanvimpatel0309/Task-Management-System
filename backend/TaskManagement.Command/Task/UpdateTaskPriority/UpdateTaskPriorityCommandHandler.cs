using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.DTO.Models.Task;

namespace TaskManagement.Command.Task.UpdateTaskPriority;

public sealed class UpdateTaskPriorityCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateTaskPriorityCommand> validator)
    : ICommandHandler<UpdateTaskPriorityCommand, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> HandleAsync(UpdateTaskPriorityCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var taskItem = await taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var updatedByUser = await userRepository.GetActiveByIdAsync(command.UpdatedByUserId, cancellationToken)
            ?? throw new NotFoundException("Active updater user was not found.");

        var nextPriority = Enum.Parse<TaskPriority>(command.Priority.Trim(), true);
        var previousPriority = taskItem.Priority;

        taskItem.Priority = nextPriority;

        taskItemRepository.Update(taskItem);
        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "TaskPriorityUpdated",
                Details = $"Task priority changed from {previousPriority} to {nextPriority}.",
                OldValue = previousPriority.ToString(),
                NewValue = nextPriority.ToString(),
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