using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Command.Comment.DeleteComment;

public sealed class DeleteCommentCommandHandler(
    ICommentRepository commentRepository,
    IUserRepository userRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteCommentCommand, bool>
{
    public async Task<bool> HandleAsync(DeleteCommentCommand command, CancellationToken cancellationToken = default)
    {
        var comment = await commentRepository.GetByIdAsync(command.CommentId, cancellationToken)
            ?? throw new NotFoundException("Comment was not found.");

        var user = await userRepository.GetActiveByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException("Active user was not found.");

        if (comment.UserId != user.Id
            && !string.Equals(command.CurrentUserRole, TaskManagement.Domain.Enums.UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase)
            && !string.Equals(command.CurrentUserRole, TaskManagement.Domain.Enums.UserRole.ProjectManager.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException("You do not have permission to delete this comment.");
        }

        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "CommentDeleted",
                Details = "A task comment was deleted.",
                OldValue = comment.Content,
                UserId = user.Id,
                TaskItemId = comment.TaskItemId
            },
            cancellationToken);

        commentRepository.Remove(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}