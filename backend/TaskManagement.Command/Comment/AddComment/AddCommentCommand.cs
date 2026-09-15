namespace TaskManagement.Command.Comment.AddComment;

public sealed class AddCommentCommand
{
    public Guid TaskId { get; init; }
    public string Content { get; init; } = string.Empty;
    public Guid UserId { get; init; }
}