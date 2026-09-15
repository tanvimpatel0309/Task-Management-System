namespace TaskManagement.Queries.Task.GetTasks;

public sealed class GetTasksQuery
{
    public string? SearchTerm { get; init; }
    public Guid? ProjectId { get; init; }
    public Guid? AssignedUserId { get; init; }
    public string? Status { get; init; }
    public string? Priority { get; init; }
    public bool? OverdueOnly { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
}