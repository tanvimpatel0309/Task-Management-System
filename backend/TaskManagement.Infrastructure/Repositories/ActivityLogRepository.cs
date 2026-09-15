using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public sealed class ActivityLogRepository(TaskManagementDbContext dbContext) : Repository<ActivityLog>(dbContext), IActivityLogRepository
{
	public async Task<IReadOnlyList<ActivityLog>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		return await DbSet.AsNoTracking()
			.Include(activityLog => activityLog.User)
			.Where(activityLog => activityLog.ProjectId == projectId)
			.OrderByDescending(activityLog => activityLog.OccurredAtUtc)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<ActivityLog>> ListByTaskItemIdAsync(Guid taskItemId, CancellationToken cancellationToken = default)
	{
		return await DbSet.AsNoTracking()
			.Include(activityLog => activityLog.User)
			.Where(activityLog => activityLog.TaskItemId == taskItemId)
			.OrderByDescending(activityLog => activityLog.OccurredAtUtc)
			.ToListAsync(cancellationToken);
	}

	public Task<int> DeleteByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		return DbSet
			.Where(activityLog => activityLog.ProjectId == projectId)
			.ExecuteDeleteAsync(cancellationToken);
	}

	public Task<int> DeleteByTaskItemIdAsync(Guid taskItemId, CancellationToken cancellationToken = default)
	{
		return DbSet
			.Where(activityLog => activityLog.TaskItemId == taskItemId)
			.ExecuteDeleteAsync(cancellationToken);
	}
}