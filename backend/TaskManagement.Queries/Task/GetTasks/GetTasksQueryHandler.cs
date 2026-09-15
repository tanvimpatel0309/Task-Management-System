using TaskManagement.AppServices.Cqrs;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Enums;
using TaskManagement.DTO.Models.Task;
using TaskManagement.DTO.Utility;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Queries.Task.GetTasks;

public sealed class GetTasksQueryHandler(ITaskItemRepository taskItemRepository)
    : IQueryHandler<GetTasksQuery, PagedResultDto<TaskListItemDto>>
{
    public async Task<PagedResultDto<TaskListItemDto>> HandleAsync(GetTasksQuery query, CancellationToken cancellationToken = default)
    {
        var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
        var pageSize = query.PageSize switch
        {
            < 1 => 20,
            > 100 => 100,
            _ => query.PageSize
        };

        DomainTaskStatus? status = string.IsNullOrWhiteSpace(query.Status)
            ? null
            : Enum.Parse<DomainTaskStatus>(query.Status.Trim(), true);

        TaskPriority? priority = string.IsNullOrWhiteSpace(query.Priority)
            ? null
            : Enum.Parse<TaskPriority>(query.Priority.Trim(), true);

        var descending = !string.Equals(query.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        var tasks = await taskItemRepository.SearchAsync(
            query.SearchTerm,
            query.ProjectId,
            query.AssignedUserId,
            status,
            priority,
            query.OverdueOnly,
            pageNumber,
            pageSize,
            query.SortBy,
            descending,
            cancellationToken);

        var totalCount = await taskItemRepository.CountSearchAsync(
            query.SearchTerm,
            query.ProjectId,
            query.AssignedUserId,
            status,
            priority,
            query.OverdueOnly,
            cancellationToken);

        return new PagedResultDto<TaskListItemDto>
        {
            Items = tasks.Select(taskItem => taskItem.ToListItemDto()).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}