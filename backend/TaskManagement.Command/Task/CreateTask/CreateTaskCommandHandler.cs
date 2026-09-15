using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.DTO.Models.Task;
using DomainUser = TaskManagement.Domain.Entities.User;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Command.Task.CreateTask;

public sealed class CreateTaskCommandHandler(
    ITaskItemRepository taskItemRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateTaskCommand> validator)
    : ICommandHandler<CreateTaskCommand, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> HandleAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var project = await projectRepository.GetByIdAsync(command.ProjectId, cancellationToken)
            ?? throw new NotFoundException("Project was not found.");

        if (!project.CanAcceptTasks())
        {
            throw new ConflictException("Archived projects cannot receive new tasks.");
        }

        var createdByUser = await userRepository.GetActiveByIdAsync(command.CreatedByUserId, cancellationToken)
            ?? throw new NotFoundException("Active creator user was not found.");

        DomainUser? assignedUser = null;
        if (command.AssignedUserId.HasValue)
        {
            assignedUser = await userRepository.GetActiveByIdAsync(command.AssignedUserId.Value, cancellationToken)
                ?? throw new NotFoundException("Active assignee user was not found.");
        }

        var status = string.IsNullOrWhiteSpace(command.Status)
            ? DomainTaskStatus.Todo
            : Enum.Parse<DomainTaskStatus>(command.Status.Trim(), true);

        var priority = string.IsNullOrWhiteSpace(command.Priority)
            ? TaskPriority.Medium
            : Enum.Parse<TaskPriority>(command.Priority.Trim(), true);

        var taskItem = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = command.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(command.Description) ? null : command.Description.Trim(),
            Status = status,
            Priority = priority,
            ProgressPercentage = status == DomainTaskStatus.Completed ? 100 : command.ProgressPercentage,
            StartDateUtc = command.StartDateUtc,
            DueDateUtc = command.DueDateUtc,
            CompletedDateUtc = status == DomainTaskStatus.Completed ? DateTime.UtcNow : null,
            ProjectId = project.Id,
            AssignedUserId = assignedUser?.Id,
            CreatedByUserId = createdByUser.Id
        };

        await taskItemRepository.AddAsync(taskItem, cancellationToken);
        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "TaskCreated",
                Details = $"Task '{taskItem.Title}' was created.",
                NewValue = status.ToString(),
                UserId = createdByUser.Id,
                ProjectId = project.Id,
                TaskItemId = taskItem.Id
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var createdTask = await taskItemRepository.GetByIdWithDetailsAsync(taskItem.Id, cancellationToken)
            ?? throw new NotFoundException("Task was not found after creation.");

        return createdTask.ToDetailsDto();
    }
}