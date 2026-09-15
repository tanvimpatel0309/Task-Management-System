using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public sealed class CommentRepository(TaskManagementDbContext dbContext) : Repository<Comment>(dbContext), ICommentRepository
{
	public Task<Comment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return DbSet.AsNoTracking()
			.Include(comment => comment.User)
			.Include(comment => comment.TaskItem)
			.FirstOrDefaultAsync(comment => comment.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<Comment>> ListByTaskItemIdAsync(Guid taskItemId, CancellationToken cancellationToken = default)
	{
		return await DbSet.AsNoTracking()
			.Include(comment => comment.User)
			.Where(comment => comment.TaskItemId == taskItemId)
			.OrderByDescending(comment => comment.CreatedAtUtc)
			.ToListAsync(cancellationToken);
	}
}