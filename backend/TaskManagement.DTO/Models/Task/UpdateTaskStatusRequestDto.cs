namespace TaskManagement.DTO.Models.Task;

public sealed class UpdateTaskStatusRequestDto
{
    public string Status { get; set; } = string.Empty;
}