namespace TaskManagement.Command.Project.CreateProject;

public sealed class CreateProjectCommand
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartDateUtc { get; init; }
    public DateTime? EndDateUtc { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid CreatedByUserId { get; init; }
}