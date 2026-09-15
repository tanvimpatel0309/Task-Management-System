using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Users;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Command.User.DeactivateUser;

public sealed class DeactivateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeactivateUserCommand, UserDetailsDto>
{
    public async Task<UserDetailsDto> HandleAsync(DeactivateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException("User was not found.");

        user.IsActive = false;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDetailsDto();
    }
}