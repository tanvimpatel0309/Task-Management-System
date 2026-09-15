using FluentValidation;

namespace TaskManagement.Command.Comment.AddComment;

public sealed class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(command => command.TaskId)
            .NotEmpty();

        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command.Content)
            .NotEmpty()
            .MaximumLength(2000);
    }
}