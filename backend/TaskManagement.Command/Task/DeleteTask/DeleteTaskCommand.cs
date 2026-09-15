namespace TaskManagement.Command.Task.DeleteTask;

public sealed class DeleteTaskCommand
{
    public Guid TaskId { get; init; }
    public Guid DeletedByUserId { get; init; }
}