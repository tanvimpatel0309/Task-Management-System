using FluentValidation;
using DomainUser = TaskManagement.Domain.Entities.User;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.AppServices.Users;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Enums;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Command.User.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUserPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IValidator<CreateUserCommand> validator)
    : ICommandHandler<CreateUserCommand, UserDetailsDto>
{
    public async Task<UserDetailsDto> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        if (await userRepository.EmailExistsAsync(normalizedEmail, cancellationToken: cancellationToken))
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var role = Enum.Parse<UserRole>(command.Role.Trim(), true);

        var user = new DomainUser
        {
            Id = Guid.NewGuid(),
            FirstName = command.FirstName.Trim(),
            LastName = command.LastName.Trim(),
            Email = normalizedEmail,
            Role = role,
            IsActive = command.IsActive
        };

        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDetailsDto();
    }
}