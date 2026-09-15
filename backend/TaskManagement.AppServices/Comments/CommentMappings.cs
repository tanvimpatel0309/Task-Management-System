using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.Comment;

namespace TaskManagement.AppServices.Comments;

public static class CommentMappings
{
    public static CommentDto ToDto(this Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            TaskItemId = comment.TaskItemId,
            UserId = comment.UserId,
            UserName = comment.User is null
                ? string.Empty
                : $"{comment.User.FirstName} {comment.User.LastName}".Trim(),
            CreatedAtUtc = comment.CreatedAtUtc,
            UpdatedAtUtc = comment.UpdatedAtUtc
        };
    }
}