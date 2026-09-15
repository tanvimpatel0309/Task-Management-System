using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Contracts;

public interface IUserRepository : IRepository<User>
{
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<bool> EmailExistsAsync(string email, Guid? excludingUserId = null, CancellationToken cancellationToken = default);
	Task<User?> GetDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<User?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<User>> ListActiveUsersAsync(CancellationToken cancellationToken = default);
	Task<IReadOnlyList<User>> ListUsersAsync(CancellationToken cancellationToken = default);
}