using FluentValidation;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Comments;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.Domain.Contracts;
using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Comment;
using DomainComment = TaskManagement.Domain.Entities.Comment;

namespace TaskManagement.Command.Comment.AddComment;

public sealed class AddCommentCommandHandler(
    ITaskItemRepository taskItemRepository,
    IUserRepository userRepository,
    ICommentRepository commentRepository,
    IActivityLogRepository activityLogRepository,
    IUnitOfWork unitOfWork,
    IValidator<AddCommentCommand> validator)
    : ICommandHandler<AddCommentCommand, CommentDto>
{
    public async Task<CommentDto> HandleAsync(AddCommentCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var taskItem = await taskItemRepository.GetByIdAsync(command.TaskId, cancellationToken)
            ?? throw new NotFoundException("Task was not found.");

        var user = await userRepository.GetActiveByIdAsync(command.UserId, cancellationToken)
            ?? throw new NotFoundException("Active user was not found.");

        var comment = new DomainComment
        {
            Id = Guid.NewGuid(),
            Content = command.Content.Trim(),
            TaskItemId = taskItem.Id,
            UserId = user.Id
        };

        await commentRepository.AddAsync(comment, cancellationToken);
        await activityLogRepository.AddAsync(
            new ActivityLog
            {
                Id = Guid.NewGuid(),
                Action = "CommentAdded",
                Details = $"A comment was added to task '{taskItem.Title}'.",
                NewValue = comment.Content,
                UserId = user.Id,
                ProjectId = taskItem.ProjectId,
                TaskItemId = taskItem.Id
            },
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var createdComment = await commentRepository.GetByIdWithDetailsAsync(comment.Id, cancellationToken)
            ?? throw new NotFoundException("Comment was not found after creation.");

        return createdComment.ToDto();
    }
}