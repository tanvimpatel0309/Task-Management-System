using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Users;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Command.User.UpdateUser;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateUserCommand> validator)
    : ICommandHandler<UpdateUserCommand, UserDetailsDto>
{
    public async Task<UserDetailsDto> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException("User was not found.");

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        if (await userRepository.EmailExistsAsync(normalizedEmail, user.Id, cancellationToken))
        {
            throw new ConflictException("A user with this email already exists.");
        }

        user.FirstName = command.FirstName.Trim();
        user.LastName = command.LastName.Trim();
        user.Email = normalizedEmail;

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDetailsDto();
    }
}