using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Command.Task.DeleteTask;

public sealed class DeleteTaskCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteTaskCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteTaskCommand command, CancellationToken cancellationToken = default)
    {
        var taskItem = await taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var deletedByUser = await userRepository.GetActiveByIdAsync(command.DeletedByUserId, cancellationToken)
            ?? throw new NotFoundException("Active deleter user was not found.");

        await activityLogRepository.DeleteByTaskItemIdAsync(taskItem.Id, cancellationToken);

        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "TaskDeleted",
                Details = $"Task '{taskItem.Title}' was deleted.",
                OldValue = taskItem.Status.ToString(),
                UserId = deletedByUser.Id,
                ProjectId = taskItem.ProjectId,
                TaskItemId = null
            },
            cancellationToken);

        taskItemRepository.Remove(taskItem);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}