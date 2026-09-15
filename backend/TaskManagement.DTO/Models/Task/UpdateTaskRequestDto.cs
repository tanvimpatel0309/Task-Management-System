namespace TaskManagement.DTO.Models.Task;

public sealed class UpdateTaskRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public Guid ProjectId { get; set; }
}