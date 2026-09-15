using FluentValidation;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Command.Task.UpdateTaskPriority;

public sealed class UpdateTaskPriorityCommandValidator : AbstractValidator<UpdateTaskPriorityCommand>
{
    public UpdateTaskPriorityCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .NotEmpty();

        RuleFor(command => command.UpdatedByUserId)
            .NotEmpty();

        RuleFor(command => command.Priority)
            .NotEmpty()
            .Must(priority => Enum.TryParse<TaskPriority>(priority, true, out _))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");
    }
}