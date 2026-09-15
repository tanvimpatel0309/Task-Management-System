using FluentValidation;

namespace TaskManagement.Command.Task.AssignTask;

public sealed class AssignTaskCommandValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .NotEmpty();

        RuleFor(command => command.UpdatedByUserId)
            .NotEmpty();

        RuleFor(command => command.AssignedUserId)
            .NotEqual(Guid.Empty)
            .When(command => command.AssignedUserId.HasValue)
            .WithMessage("Assigned user id must be a valid value.");
    }
}