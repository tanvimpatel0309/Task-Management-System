using FluentValidation;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Command.Project.CreateProject;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(2000);

        RuleFor(command => command.StartDateUtc)
            .NotEmpty();

        RuleFor(command => command.CreatedByUserId)
            .NotEmpty();

        RuleFor(command => command.EndDateUtc)
            .GreaterThanOrEqualTo(command => command.StartDateUtc)
            .When(command => command.EndDateUtc.HasValue)
            .WithMessage("End date must be greater than or equal to the start date.");

        RuleFor(command => command.Status)
            .Must(BeAValidStatus)
            .When(command => !string.IsNullOrWhiteSpace(command.Status))
            .WithMessage("Status must be one of: Planning, Active, Completed, Archived.");
    }

    private static bool BeAValidStatus(string status)
    {
        return Enum.TryParse<ProjectStatus>(status, true, out _);
    }
}