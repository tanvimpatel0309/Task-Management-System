using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Task;

namespace TaskManagement.Command.Task.AssignTask;

public sealed class AssignTaskCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IValidator<AssignTaskCommand> validator)
    : ICommandHandler<AssignTaskCommand, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> HandleAsync(AssignTaskCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var taskItem = await taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var updatedByUser = await userRepository.GetActiveByIdAsync(command.UpdatedByUserId, cancellationToken)
            ?? throw new NotFoundException("Active updater user was not found.");

        var previousAssignedUserId = taskItem.AssignedUserId;
        string previousAssignedUserName = string.Empty;
        if (previousAssignedUserId.HasValue)
        {
            var previousAssignedUser = await userRepository.GetActiveByIdAsync(previousAssignedUserId.Value, cancellationToken);
            previousAssignedUserName = previousAssignedUser is null
                ? previousAssignedUserId.Value.ToString()
                : $"{previousAssignedUser.FirstName} {previousAssignedUser.LastName}".Trim();
        }

        string nextAssignedUserName = "Unassigned";
        if (command.AssignedUserId.HasValue)
        {
            var assignedUser = await userRepository.GetActiveByIdAsync(command.AssignedUserId.Value, cancellationToken)
                ?? throw new NotFoundException("Active assignee user was not found.");

            taskItem.AssignedUserId = assignedUser.Id;
            nextAssignedUserName = $"{assignedUser.FirstName} {assignedUser.LastName}".Trim();
        }
        else
        {
            taskItem.AssignedUserId = null;
        }

        taskItemRepository.Update(taskItem);
        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "TaskAssignmentUpdated",
                Details = $"Task assignment changed from {previousAssignedUserName} to {nextAssignedUserName}.",
                OldValue = previousAssignedUserName,
                NewValue = nextAssignedUserName,
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