using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Contracts;

public interface ITaskItemRepository : IRepository<TaskItem>
{
	Task<TaskItem?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<TaskItem>> ListByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<TaskItem>> SearchAsync(
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
		CancellationToken cancellationToken = default);
	Task<int> CountSearchAsync(
		string? searchTerm,
		Guid? projectId,
		Guid? assignedUserId,
		DomainTaskStatus? status,
		TaskPriority? priority,
		bool? overdueOnly,
		CancellationToken cancellationToken = default);
}