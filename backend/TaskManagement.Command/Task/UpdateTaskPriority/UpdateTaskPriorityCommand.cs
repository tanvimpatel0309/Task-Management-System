namespace TaskManagement.Command.Task.UpdateTaskPriority;

public sealed class UpdateTaskPriorityCommand
{
    public Guid TaskId { get; init; }
    public string Priority { get; init; } = string.Empty;
    public Guid UpdatedByUserId { get; init; }
}