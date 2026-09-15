namespace TaskManagement.Command.Comment.UpdateComment;

public sealed class UpdateCommentCommand
{
    public Guid CommentId { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string CurrentUserRole { get; init; } = string.Empty;
}