using FluentValidation;

namespace TaskManagement.Command.Task.UpdateTask;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(2000);

        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.UpdatedByUserId)
            .NotEmpty();

        RuleFor(command => command.DueDateUtc)
            .GreaterThanOrEqualTo(command => command.StartDateUtc)
            .When(command => command.StartDateUtc.HasValue && command.DueDateUtc.HasValue)
            .WithMessage("Due date must be greater than or equal to the start date.");
    }
}