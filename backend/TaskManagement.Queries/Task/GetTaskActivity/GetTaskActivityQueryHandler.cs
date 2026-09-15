using TaskManagement.AppServices.Cqrs;
using TaskManagement.AppServices.Tasks;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.Activity;

namespace TaskManagement.Queries.Task.GetTaskActivity;

public sealed class GetTaskActivityQueryHandler(IActivityLogRepository activityLogRepository)
    : IQueryHandler<GetTaskActivityQuery, IReadOnlyList<ActivityLogDto>>
{
    public async Task<IReadOnlyList<ActivityLogDto>> HandleAsync(GetTaskActivityQuery query, CancellationToken cancellationToken = default)
    {
        var activityLogs = await activityLogRepository.ListByTaskItemIdAsync(query.TaskId, cancellationToken);
        return activityLogs.Select(activityLog => activityLog.ToDto()).ToList();
    }
}