using FluentValidation;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Command.User.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(command => command.Role)
            .NotEmpty()
            .Must(BeAValidRole)
            .WithMessage("Role must be one of: Admin, ProjectManager, TeamMember.");
    }

    private static bool BeAValidRole(string role)
    {
        return Enum.TryParse<UserRole>(role, true, out _);
    }
}