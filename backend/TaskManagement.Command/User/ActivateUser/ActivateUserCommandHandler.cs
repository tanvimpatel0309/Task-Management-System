using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Users;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Command.User.ActivateUser;

public sealed class ActivateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ActivateUserCommand, UserDetailsDto>
{
    public async Task<UserDetailsDto> HandleAsync(ActivateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException("User was not found.");

        user.IsActive = true;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDetailsDto();
    }
}