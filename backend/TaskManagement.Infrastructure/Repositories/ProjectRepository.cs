using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public sealed class ProjectRepository(TaskManagementDbContext dbContext) : Repository<Project>(dbContext), IProjectRepository
{
	public Task<Project?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return DbSet.AsNoTracking()
			.Include(project => project.CreatedByUser)
			.Include(project => project.Tasks)
			.FirstOrDefaultAsync(project => project.Id == id, cancellationToken);
	}

	public Task<Project?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return DbSet.AsNoTracking()
			.Include(project => project.CreatedByUser)
			.Include(project => project.Tasks)
			.FirstOrDefaultAsync(project => project.Id == id, cancellationToken);
	}

	public async Task<IReadOnlyList<Project>> ListProjectsAsync(CancellationToken cancellationToken = default)
	{
		return await DbSet.AsNoTracking()
			.Include(project => project.CreatedByUser)
			.Include(project => project.Tasks)
			.OrderBy(project => project.Name)
			.ToListAsync(cancellationToken);
	}
}