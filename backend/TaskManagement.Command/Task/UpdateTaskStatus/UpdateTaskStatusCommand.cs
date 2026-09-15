namespace TaskManagement.Command.Task.UpdateTaskStatus;

public sealed class UpdateTaskStatusCommand
{
    public Guid TaskId { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid UpdatedByUserId { get; init; }
}