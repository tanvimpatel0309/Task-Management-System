using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Comments;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Comment;

namespace TaskManagement.Command.Comment.UpdateComment;

public sealed class UpdateCommentCommandHandler(
    ICommentRepository commentRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateCommentCommand> validator)
    : ICommandHandler<UpdateCommentCommand, CommentDto>
{
    public async Task<CommentDto> HandleAsync(UpdateCommentCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var comment = await commentRepository.GetByIdAsync(command.CommentId, cancellationToken)
            ?? throw new NotFoundException("Comment was not found.");

        var user = await userRepository.GetActiveByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException("Active user was not found.");

        if (comment.UserId != user.Id
            && !string.Equals(command.CurrentUserRole, TaskManagement.Domain.Enums.UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase)
            && !string.Equals(command.CurrentUserRole, TaskManagement.Domain.Enums.UserRole.ProjectManager.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException("You do not have permission to edit this comment.");
        }

        var previousContent = comment.Content;
        comment.Content = command.Content.Trim();

        commentRepository.Update(comment);
        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "CommentUpdated",
                Details = "A task comment was updated.",
                OldValue = previousContent,
                NewValue = comment.Content,
                UserId = user.Id,
                TaskItemId = comment.TaskItemId
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedComment = await commentRepository.GetByIdWithDetailsAsync(comment.Id, cancellationToken)
            ?? throw new NotFoundException("Comment was not found.");

        return updatedComment.ToDto();
    }
}