using TaskManagement.Domain.Entities.Common;

namespace TaskManagement.Domain.Entities;

public sealed class ActivityLog : BaseEntity
{
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public Guid UserId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? TaskItemId { get; set; }

    public User? User { get; set; }
    public Project? Project { get; set; }
    public TaskItem? TaskItem { get; set; }
}