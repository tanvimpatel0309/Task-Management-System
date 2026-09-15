using TaskManagement.Domain.Entities.Common;

namespace TaskManagement.Domain.Entities;

public sealed class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid TaskItemId { get; set; }
    public Guid UserId { get; set; }

    public TaskItem? TaskItem { get; set; }
    public User? User { get; set; }
}