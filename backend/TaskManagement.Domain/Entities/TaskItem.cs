using TaskManagement.Domain.Entities.Common;
using TaskManagement.Domain.Enums;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

public sealed class TaskItem : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DomainTaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int ProgressPercentage { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public DateTime? CompletedDateUtc { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedUserId { get; set; }
    public Guid CreatedByUserId { get; set; }

    public Project? Project { get; set; }
    public User? AssignedUser { get; set; }
    public User? CreatedByUser { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
}