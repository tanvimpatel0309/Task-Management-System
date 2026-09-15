namespace TaskManagement.Command.Project.UpdateProject;

public sealed class UpdateProjectCommand
{
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartDateUtc { get; init; }
    public DateTime? EndDateUtc { get; init; }
}