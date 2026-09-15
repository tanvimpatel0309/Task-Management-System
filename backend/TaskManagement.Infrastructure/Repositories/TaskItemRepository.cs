using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Infrastructure.Repositories;

public sealed class TaskItemRepository(TaskManagementDbContext dbContext) : Repository<TaskItem>(dbContext), ITaskItemRepository
{
	public Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return IncludeDetails(DbSet.AsNoTracking())
			.FirstOrDefaultAsync(taskItem => taskItem.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<TaskItem>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
	{
		return await DbSet
			.Where(taskItem => taskItem.ProjectId == projectId)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<TaskItem>> SearchAsync(
		string? searchTerm,
		Guid? projectId,
		Guid? assignedUserId,
		DomainTaskStatus? status,
		TaskPriority? priority,
		bool? overdueOnly,
		int pageNumber,
		int pageSize,
		string? sortBy,
		bool descending,
		CancellationToken cancellationToken = default)
	{
		var query = ApplySorting(
			ApplyFilters(IncludeDetails(DbSet.AsNoTracking()), searchTerm, projectId, assignedUserId, status, priority, overdueOnly),
			sortBy,
			descending);

		return await query
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
	}

	public Task<int> CountSearchAsync(
		string? searchTerm,
		Guid? projectId,
		Guid? assignedUserId,
		DomainTaskStatus? status,
		TaskPriority? priority,
		bool? overdueOnly,
		CancellationToken cancellationToken = default)
	{
		return ApplyFilters(DbSet.AsNoTracking(), searchTerm, projectId, assignedUserId, status, priority, overdueOnly)
			.CountAsync(cancellationToken);
	}

	private static IQueryable<TaskItem> IncludeDetails(IQueryable<TaskItem> query)
	{
		return query
			.Include(taskItem => taskItem.Project)
			.Include(taskItem => taskItem.AssignedUser)
			.Include(taskItem => taskItem.CreatedByUser)
			.Include(taskItem => taskItem.Comments)
			.Include(taskItem => taskItem.ActivityLogs);
	}

	private static IQueryable<TaskItem> ApplyFilters(
		IQueryable<TaskItem> query,
		string? searchTerm,
		Guid? projectId,
		Guid? assignedUserId,
		DomainTaskStatus? status,
		TaskPriority? priority,
		bool? overdueOnly)
	{
		if (!string.IsNullOrWhiteSpace(searchTerm))
		{
			var trimmedSearchTerm = searchTerm.Trim();
			query = query.Where(taskItem =>
				taskItem.Title.Contains(trimmedSearchTerm)
				|| (taskItem.Description != null && taskItem.Description.Contains(trimmedSearchTerm))
				|| taskItem.Project!.Name.Contains(trimmedSearchTerm)
				|| (taskItem.AssignedUser != null
					&& ((taskItem.AssignedUser.FirstName + " " + taskItem.AssignedUser.LastName).Contains(trimmedSearchTerm)
						|| taskItem.AssignedUser.FirstName.Contains(trimmedSearchTerm)
						|| taskItem.AssignedUser.LastName.Contains(trimmedSearchTerm))));
		}

		if (projectId.HasValue)
		{
			query = query.Where(taskItem => taskItem.ProjectId == projectId.Value);
		}

		if (assignedUserId.HasValue)
		{
			query = query.Where(taskItem => taskItem.AssignedUserId == assignedUserId.Value);
		}

		if (status.HasValue)
		{
			query = query.Where(taskItem => taskItem.Status == status.Value);
		}

		if (priority.HasValue)
		{
			query = query.Where(taskItem => taskItem.Priority == priority.Value);
		}

		if (overdueOnly == true)
		{
			var utcNow = DateTime.UtcNow;
			query = query.Where(taskItem =>
				taskItem.DueDateUtc.HasValue
				&& taskItem.DueDateUtc.Value < utcNow
				&& taskItem.Status != DomainTaskStatus.Completed
				&& taskItem.Status != DomainTaskStatus.Cancelled);
		}

		return query;
	}

	private static IQueryable<TaskItem> ApplySorting(IQueryable<TaskItem> query, string? sortBy, bool descending)
	{
		return (sortBy ?? string.Empty).Trim().ToLowerInvariant() switch
		{
			"title" => descending ? query.OrderByDescending(taskItem => taskItem.Title) : query.OrderBy(taskItem => taskItem.Title),
			"status" => descending ? query.OrderByDescending(taskItem => taskItem.Status) : query.OrderBy(taskItem => taskItem.Status),
			"priority" => descending ? query.OrderByDescending(taskItem => taskItem.Priority) : query.OrderBy(taskItem => taskItem.Priority),
			"progress" => descending ? query.OrderByDescending(taskItem => taskItem.ProgressPercentage) : query.OrderBy(taskItem => taskItem.ProgressPercentage),
			"duedate" or "duedateutc" => descending ? query.OrderByDescending(taskItem => taskItem.DueDateUtc) : query.OrderBy(taskItem => taskItem.DueDateUtc),
			"updatedat" or "updatedatutc" => descending ? query.OrderByDescending(taskItem => taskItem.UpdatedAtUtc) : query.OrderBy(taskItem => taskItem.UpdatedAtUtc),
			"createdat" or "createdatutc" => descending ? query.OrderByDescending(taskItem => taskItem.CreatedAtUtc) : query.OrderBy(taskItem => taskItem.CreatedAtUtc),
			_ => descending ? query.OrderByDescending(taskItem => taskItem.CreatedAtUtc) : query.OrderBy(taskItem => taskItem.CreatedAtUtc)
		};
	}
}