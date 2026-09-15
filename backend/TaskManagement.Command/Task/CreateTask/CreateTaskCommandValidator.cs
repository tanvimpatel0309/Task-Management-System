using FluentValidation;
using TaskManagement.Domain.Enums;
using DomainTaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Command.Task.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Description)
            .MaximumLength(2000);

        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.CreatedByUserId)
            .NotEmpty();

        RuleFor(command => command.ProgressPercentage)
            .InclusiveBetween(0, 100);

        RuleFor(command => command.DueDateUtc)
            .GreaterThanOrEqualTo(command => command.StartDateUtc)
            .When(command => command.StartDateUtc.HasValue && command.DueDateUtc.HasValue)
            .WithMessage("Due date must be greater than or equal to the start date.");

        RuleFor(command => command.Status)
            .Must(BeAValidStatus)
            .When(command => !string.IsNullOrWhiteSpace(command.Status))
            .WithMessage("Status must be one of: Todo, InProgress, OnHold, Completed, Cancelled.");

        RuleFor(command => command.Priority)
            .Must(BeAValidPriority)
            .When(command => !string.IsNullOrWhiteSpace(command.Priority))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");
    }

    private static bool BeAValidStatus(string status)
    {
        return Enum.TryParse<DomainTaskStatus>(status, true, out _);
    }

    private static bool BeAValidPriority(string priority)
    {
        return Enum.TryParse<TaskPriority>(priority, true, out _);
    }
}