using FluentValidation;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Command.Project.UpdateProjectStatus;

public sealed class UpdateProjectStatusCommandValidator : AbstractValidator<UpdateProjectStatusCommand>
{
    public UpdateProjectStatusCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.Status)
            .NotEmpty()
            .Must(BeAValidStatus)
            .WithMessage("Status must be one of: Planning, Active, Completed, Archived.");
    }

    private static bool BeAValidStatus(string status)
    {
        return Enum.TryParse<ProjectStatus>(status, true, out _);
    }
}