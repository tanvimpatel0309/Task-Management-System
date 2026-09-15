namespace TaskManagement.Command.Task.CreateTask;

public sealed class CreateTaskCommand
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public int ProgressPercentage { get; init; }
    public DateTime? StartDateUtc { get; init; }
    public DateTime? DueDateUtc { get; init; }
    public Guid ProjectId { get; init; }
    public Guid? AssignedUserId { get; init; }
    public Guid CreatedByUserId { get; init; }
}