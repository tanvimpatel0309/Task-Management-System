using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.DTO.Models.Task;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.AppServices.Tasks;

public static class TaskMappings
{
    public static TaskListItemDto ToListItemDto(this TaskItem taskItem)
    {
        return new TaskListItemDto
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status.ToString(),
            Priority = taskItem.Priority.ToString(),
            ProgressPercentage = taskItem.ProgressPercentage,
            DueDateUtc = taskItem.DueDateUtc,
            ProjectId = taskItem.ProjectId,
            ProjectName = taskItem.Project?.Name ?? string.Empty,
            AssignedUserId = taskItem.AssignedUserId,
            AssignedUserName = taskItem.AssignedUser is null
                ? string.Empty
                : $"{taskItem.AssignedUser.FirstName} {taskItem.AssignedUser.LastName}".Trim(),
            IsOverdue = IsOverdue(taskItem),
            CreatedAtUtc = taskItem.CreatedAtUtc,
            UpdatedAtUtc = taskItem.UpdatedAtUtc
        };
    }

    public static TaskDetailsDto ToDetailsDto(this TaskItem taskItem)
    {
        return new TaskDetailsDto
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Description = taskItem.Description,
            Status = taskItem.Status.ToString(),
            Priority = taskItem.Priority.ToString(),
            ProgressPercentage = taskItem.ProgressPercentage,
            StartDateUtc = taskItem.StartDateUtc,
            DueDateUtc = taskItem.DueDateUtc,
            CompletedDateUtc = taskItem.CompletedDateUtc,
            ProjectId = taskItem.ProjectId,
            ProjectName = taskItem.Project?.Name ?? string.Empty,
            AssignedUserId = taskItem.AssignedUserId,
            AssignedUserName = taskItem.AssignedUser is null
                ? string.Empty
                : $"{taskItem.AssignedUser.FirstName} {taskItem.AssignedUser.LastName}".Trim(),
            CreatedByUserId = taskItem.CreatedByUserId,
            CreatedByUserName = taskItem.CreatedByUser is null
                ? string.Empty
                : $"{taskItem.CreatedByUser.FirstName} {taskItem.CreatedByUser.LastName}".Trim(),
            CommentCount = taskItem.Comments.Count,
            ActivityCount = taskItem.ActivityLogs.Count,
            IsOverdue = IsOverdue(taskItem),
            CreatedAtUtc = taskItem.CreatedAtUtc,
            UpdatedAtUtc = taskItem.UpdatedAtUtc
        };
    }

    private static bool IsOverdue(TaskItem taskItem)
    {
        return taskItem.DueDateUtc.HasValue
            && taskItem.DueDateUtc.Value < DateTime.UtcNow
            && taskItem.Status != DomainTaskStatus.Completed
            && taskItem.Status != DomainTaskStatus.Cancelled;
    }
}