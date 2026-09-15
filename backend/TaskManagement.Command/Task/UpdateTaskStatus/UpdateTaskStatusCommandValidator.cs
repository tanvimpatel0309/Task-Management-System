using FluentValidation;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Command.Task.UpdateTaskStatus;

public sealed class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .NotEmpty();

        RuleFor(command => command.UpdatedByUserId)
            .NotEmpty();

        RuleFor(command => command.Status)
            .NotEmpty()
            .Must(status => Enum.TryParse<DomainTaskStatus>(status, true, out _))
            .WithMessage("Status must be one of: Todo, InProgress, OnHold, Completed, Cancelled.");
    }
}