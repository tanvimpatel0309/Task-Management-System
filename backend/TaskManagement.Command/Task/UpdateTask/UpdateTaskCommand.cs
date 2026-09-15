namespace TaskManagement.Command.Task.UpdateTask;

public sealed class UpdateTaskCommand
{
    public Guid TaskId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? StartDateUtc { get; init; }
    public DateTime? DueDateUtc { get; init; }
    public Guid ProjectId { get; init; }
    public Guid UpdatedByUserId { get; init; }
}