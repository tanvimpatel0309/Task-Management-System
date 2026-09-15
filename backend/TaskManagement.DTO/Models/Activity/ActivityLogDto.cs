namespace TaskManagement.DTO.Models.Activity;

public sealed class ActivityLogDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime OccurredAtUtc { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}