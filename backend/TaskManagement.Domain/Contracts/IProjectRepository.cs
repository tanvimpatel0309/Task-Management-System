using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Contracts;

public interface IProjectRepository : IRepository<Project>
{
	Task<Project?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default);
	Task<Project?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Project>> ListProjectsAsync(CancellationToken cancellationToken = default);
}