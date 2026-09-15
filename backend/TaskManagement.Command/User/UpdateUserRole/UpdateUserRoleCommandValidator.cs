using FluentValidation;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Command.User.UpdateUserRole;

public sealed class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty();

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