using TaskManagement.AppServices.Comments;
using TaskManagement.AppServices.Cqrs;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.Comment;

namespace TaskManagement.Queries.Comment.GetCommentsByTask;

public sealed class GetCommentsByTaskQueryHandler(ICommentRepository commentRepository)
    : IQueryHandler<GetCommentsByTaskQuery, IReadOnlyList<CommentDto>>
{
    public async Task<IReadOnlyList<CommentDto>> HandleAsync(GetCommentsByTaskQuery query, CancellationToken cancellationToken = default)
    {
        var comments = await commentRepository.ListByTaskItemIdAsync(query.TaskId, cancellationToken);
        return comments.Select(comment => comment.ToDto()).ToList();
    }
}