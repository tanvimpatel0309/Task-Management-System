using FluentValidation;

namespace TaskManagement.Command.Task.UpdateTaskProgress;

public sealed class UpdateTaskProgressCommandValidator : AbstractValidator<UpdateTaskProgressCommand>
{
    public UpdateTaskProgressCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .NotEmpty();

        RuleFor(command => command.UpdatedByUserId)
            .NotEmpty();

        RuleFor(command => command.ProgressPercentage)
            .InclusiveBetween(0, 100);
    }
}