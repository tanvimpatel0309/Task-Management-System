using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Contracts;

public interface IActivityLogRepository : IRepository<ActivityLog>
{
	Task<IReadOnlyList<ActivityLog>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<ActivityLog>> ListByTaskItemIdAsync(Guid taskItemId, CancellationToken cancellationToken = default);
	Task<int> DeleteByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
	Task<int> DeleteByTaskItemIdAsync(Guid taskItemId, CancellationToken cancellationToken = default);
}