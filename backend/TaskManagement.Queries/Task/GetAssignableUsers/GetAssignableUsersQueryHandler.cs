using TaskManagement.AppServices.Cqrs;
using TaskManagement.AppServices.Users;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Queries.Task.GetAssignableUsers;

public sealed class GetAssignableUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetAssignableUsersQuery, IReadOnlyList<UserListItemDto>>
{
    public async Task<IReadOnlyList<UserListItemDto>> HandleAsync(GetAssignableUsersQuery query, CancellationToken cancellationToken = default)
    {
        var users = await userRepository.ListActiveUsersAsync(cancellationToken);
        return users.Select(user => user.ToListItemDto()).ToList();
    }
}