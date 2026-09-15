namespace TaskManagement.Command.Task.AssignTask;

public sealed class AssignTaskCommand
{
    public Guid TaskId { get; init; }
    public Guid? AssignedUserId { get; init; }
    public Guid UpdatedByUserId { get; init; }
}