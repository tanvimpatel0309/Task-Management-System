using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Users;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Enums;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Command.User.UpdateUserRole;

public sealed class UpdateUserRoleCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateUserRoleCommand> validator)
    : ICommandHandler<UpdateUserRoleCommand, UserDetailsDto>
{
    public async Task<UserDetailsDto> HandleAsync(UpdateUserRoleCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException("User was not found.");

        user.Role = Enum.Parse<UserRole>(command.Role.Trim(), true);

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDetailsDto();
    }
}