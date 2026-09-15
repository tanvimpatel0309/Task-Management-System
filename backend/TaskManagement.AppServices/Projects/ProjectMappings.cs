using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Project;

namespace TaskManagement.AppServices.Projects;

public static class ProjectMappings
{
    public static ProjectListItemDto ToListItemDto(this Project project)
    {
        return new ProjectListItemDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status.ToString(),
            StartDateUtc = project.StartDateUtc,
            EndDateUtc = project.EndDateUtc,
            CreatedByUserId = project.CreatedByUserId,
            CreatedByUserName = project.CreatedByUser is null
                ? string.Empty
                : $"{project.CreatedByUser.FirstName} {project.CreatedByUser.LastName}".Trim(),
            TaskCount = project.Tasks.Count,
            CreatedAtUtc = project.CreatedAtUtc,
            UpdatedAtUtc = project.UpdatedAtUtc
        };
    }

    public static ProjectDetailsDto ToDetailsDto(this Project project)
    {
        return new ProjectDetailsDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status.ToString(),
            StartDateUtc = project.StartDateUtc,
            EndDateUtc = project.EndDateUtc,
            CreatedByUserId = project.CreatedByUserId,
            CreatedByUserName = project.CreatedByUser is null
                ? string.Empty
                : $"{project.CreatedByUser.FirstName} {project.CreatedByUser.LastName}".Trim(),
            TaskCount = project.Tasks.Count,
            CanAcceptTasks = project.CanAcceptTasks(),
            CreatedAtUtc = project.CreatedAtUtc,
            UpdatedAtUtc = project.UpdatedAtUtc
        };
    }
}