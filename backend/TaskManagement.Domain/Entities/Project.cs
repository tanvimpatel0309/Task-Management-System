using TaskManagement.Domain.Entities.Common;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public sealed class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public Guid CreatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    public bool CanAcceptTasks()
    {
        return Status != ProjectStatus.Archived;
    }

    public bool CanTransitionTo(ProjectStatus nextStatus)
    {
        if (Status == nextStatus)
        {
            return true;
        }

        return Status switch
        {
            ProjectStatus.Planning => nextStatus is ProjectStatus.Active or ProjectStatus.Archived,
            ProjectStatus.Active => nextStatus is ProjectStatus.Completed or ProjectStatus.Archived,
            ProjectStatus.Completed => nextStatus == ProjectStatus.Archived,
            ProjectStatus.Archived => false,
            _ => false
        };
    }
}