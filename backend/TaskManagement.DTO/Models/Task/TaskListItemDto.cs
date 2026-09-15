namespace TaskManagement.DTO.Models.Task;

public sealed class TaskListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public Guid? AssignedUserId { get; set; }
    public string AssignedUserName { get; set; } = string.Empty;
    public bool IsOverdue { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}