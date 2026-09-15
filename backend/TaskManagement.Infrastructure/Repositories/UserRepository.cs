using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public sealed class UserRepository(TaskManagementDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
	public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return DbSet.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
	}

	public Task<bool> EmailExistsAsync(string email, Guid? excludingUserId = null, CancellationToken cancellationToken = default)
	{
		return DbSet.AnyAsync(
			user => user.Email == email && (!excludingUserId.HasValue || user.Id != excludingUserId.Value),
			cancellationToken);
	}

	public Task<User?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return DbSet.AsNoTracking()
			.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
	}

	public Task<User?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default)
	{
		return DbSet.AsNoTracking()
			.FirstOrDefaultAsync(user => user.Id == id && user.IsActive, cancellationToken);
	}

	public async Task<IReadOnlyList<User>> ListActiveUsersAsync(CancellationToken cancellationToken = default)
	{
		return await DbSet.AsNoTracking()
			.Where(user => user.IsActive)
			.OrderBy(user => user.LastName)
			.ThenBy(user => user.FirstName)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<User>> ListUsersAsync(CancellationToken cancellationToken = default)
	{
		return await DbSet.AsNoTracking()
			.OrderBy(user => user.LastName)
			.ThenBy(user => user.FirstName)
			.ToListAsync(cancellationToken);
	}
}