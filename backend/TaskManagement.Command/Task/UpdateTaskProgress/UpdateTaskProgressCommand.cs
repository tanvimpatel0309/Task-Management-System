namespace TaskManagement.Command.Task.UpdateTaskProgress;

public sealed class UpdateTaskProgressCommand
{
    public Guid TaskId { get; init; }
    public int ProgressPercentage { get; init; }
    public Guid UpdatedByUserId { get; init; }
}