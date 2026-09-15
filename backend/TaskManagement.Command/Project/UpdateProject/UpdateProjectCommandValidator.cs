using FluentValidation;

namespace TaskManagement.Command.Project.UpdateProject;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(2000);

        RuleFor(command => command.StartDateUtc)
            .NotEmpty();

        RuleFor(command => command.EndDateUtc)
            .GreaterThanOrEqualTo(command => command.StartDateUtc)
            .When(command => command.EndDateUtc.HasValue)
            .WithMessage("End date must be greater than or equal to the start date.");
    }
}