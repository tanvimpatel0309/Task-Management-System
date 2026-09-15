namespace TaskManagement.Command.Project.UpdateProjectStatus;

public sealed class UpdateProjectStatusCommand
{
    public Guid ProjectId { get; init; }
    public string Status { get; init; } = string.Empty;
}