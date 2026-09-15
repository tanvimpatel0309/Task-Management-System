namespace TaskManagement.DTO.Models.Project;

public sealed class CreateProjectRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }
    public string Status { get; set; } = string.Empty;
}