namespace TaskManagement.Command.Comment.DeleteComment;

public sealed class DeleteCommentCommand
{
    public Guid CommentId { get; init; }
    public Guid UserId { get; init; }
    public string CurrentUserRole { get; init; } = string.Empty;
}