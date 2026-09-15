using TaskManagement.AppServices.Cqrs;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Users;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Queries.User.GetUserById;

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, UserDetailsDto>
{
    public async Task<UserDetailsDto> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetDetailsByIdAsync(query.UserId, cancellationToken)
            ?? throw new NotFoundException("User was not found.");

        return user.ToDetailsDto();
    }
}