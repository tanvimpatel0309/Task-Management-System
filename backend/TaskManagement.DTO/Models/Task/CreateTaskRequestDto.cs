namespace TaskManagement.DTO.Models.Task;

public sealed class CreateTaskRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public int ProgressPercentage { get; set; }
    public DateTime? StartDateUtc { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? AssignedUserId { get; set; }
}