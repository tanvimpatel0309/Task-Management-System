using TaskManagement.AppServices.Cqrs;
using TaskManagement.AppServices.Users;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Queries.User.GetUsers;

public sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUsersQuery, IReadOnlyList<UserListItemDto>>
{
    public async Task<IReadOnlyList<UserListItemDto>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        var users = await userRepository.ListUsersAsync(cancellationToken);
        return users.Select(user => user.ToListItemDto()).ToList();
    }
}