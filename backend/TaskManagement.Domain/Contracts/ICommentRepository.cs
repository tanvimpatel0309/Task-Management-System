using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Contracts;

public interface ICommentRepository : IRepository<Comment>
{
	Task<Comment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Comment>> ListByTaskItemIdAsync(Guid taskItemId, CancellationToken cancellationToken = default);
}