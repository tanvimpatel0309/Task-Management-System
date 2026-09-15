using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Activity;

namespace TaskManagement.AppServices.Tasks;

public static class TaskActivityMappings
{
    public static ActivityLogDto ToDto(this ActivityLog activityLog)
    {
        return new ActivityLogDto
        {
            Id = activityLog.Id,
            Action = activityLog.Action,
            Details = activityLog.Details,
            OldValue = activityLog.OldValue,
            NewValue = activityLog.NewValue,
            OccurredAtUtc = activityLog.OccurredAtUtc,
            UserId = activityLog.UserId,
            UserName = activityLog.User is null
                ? string.Empty
                : $"{activityLog.User.FirstName} {activityLog.User.LastName}".Trim()
        };
    }
}