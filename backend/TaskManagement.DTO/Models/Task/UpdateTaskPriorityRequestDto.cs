namespace TaskManagement.DTO.Models.Task;

public sealed class UpdateTaskPriorityRequestDto
{
    public string Priority { get; set; } = string.Empty;
}