using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Cqrs;
using TaskManagement.Command.Comment.AddComment;
using TaskManagement.Command.Comment.DeleteComment;
using TaskManagement.Command.Comment.UpdateComment;
using TaskManagement.Command.Task.AssignTask;
using TaskManagement.Command.Task.CreateTask;
using TaskManagement.Command.Task.DeleteTask;
using TaskManagement.Command.Task.UpdateTask;
using TaskManagement.Command.Task.UpdateTaskPriority;
using TaskManagement.Command.Task.UpdateTaskProgress;
using TaskManagement.Command.Task.UpdateTaskStatus;
using TaskManagement.DTO.Models.Activity;
using TaskManagement.DTO.Models.Comment;
using TaskManagement.DTO.Models.Task;
using TaskManagement.DTO.Models.User;
using TaskManagement.DTO.Utility;
using TaskManagement.Queries.Comment.GetCommentsByTask;
using TaskManagement.Queries.Task.GetAssignableUsers;
using TaskManagement.Queries.Task.GetTaskActivity;
using TaskManagement.Queries.Task.GetTaskById;
using TaskManagement.Queries.Task.GetTasks;

namespace TaskManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.AnyTeamMember)]
public sealed class TasksController(
    ICommandHandler<CreateTaskCommand, TaskDetailsDto> createTaskCommandHandler,
    ICommandHandler<UpdateTaskCommand, TaskDetailsDto> updateTaskCommandHandler,
    ICommandHandler<UpdateTaskStatusCommand, TaskDetailsDto> updateTaskStatusCommandHandler,
    ICommandHandler<UpdateTaskPriorityCommand, TaskDetailsDto> updateTaskPriorityCommandHandler,
    ICommandHandler<UpdateTaskProgressCommand, TaskDetailsDto> updateTaskProgressCommandHandler,
    ICommandHandler<AssignTaskCommand, TaskDetailsDto> assignTaskCommandHandler,
    ICommandHandler<DeleteTaskCommand, bool> deleteTaskCommandHandler,
    ICommandHandler<AddCommentCommand, CommentDto> addCommentCommandHandler,
    ICommandHandler<UpdateCommentCommand, CommentDto> updateCommentCommandHandler,
    ICommandHandler<DeleteCommentCommand, bool> deleteCommentCommandHandler,
    IQueryHandler<GetAssignableUsersQuery, IReadOnlyList<UserListItemDto>> getAssignableUsersQueryHandler,
    IQueryHandler<GetTasksQuery, PagedResultDto<TaskListItemDto>> getTasksQueryHandler,
    IQueryHandler<GetTaskByIdQuery, TaskDetailsDto> getTaskByIdQueryHandler,
    IQueryHandler<GetCommentsByTaskQuery, IReadOnlyList<CommentDto>> getCommentsByTaskQueryHandler,
    IQueryHandler<GetTaskActivityQuery, IReadOnlyList<ActivityLogDto>> getTaskActivityQueryHandler) : ControllerBase
{
    [HttpGet("assignable-users")]
    [Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
    [ProducesResponseType(typeof(IReadOnlyList<UserListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> GetAssignableUsers(CancellationToken cancellationToken)
    {
        var response = await getAssignableUsersQueryHandler.HandleAsync(new GetAssignableUsersQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<TaskListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResultDto<TaskListItemDto>>> GetTasks(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? projectId,
        [FromQuery] Guid? assignedUserId,
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] bool? overdueOnly,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var response = await getTasksQueryHandler.HandleAsync(
            new GetTasksQuery
            {
                SearchTerm = searchTerm,
                ProjectId = projectId,
                AssignedUserId = assignedUserId,
                Status = status,
                Priority = priority,
                OverdueOnly = overdueOnly,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDirection = sortDirection
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailsDto>> GetTaskById(Guid taskId, CancellationToken cancellationToken)
    {
        var response = await getTaskByIdQueryHandler.HandleAsync(new GetTaskByIdQuery { TaskId = taskId }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TaskDetailsDto>> CreateTask(
        [FromBody] CreateTaskRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await createTaskCommandHandler.HandleAsync(
            new CreateTaskCommand
            {
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Priority = request.Priority,
                ProgressPercentage = request.ProgressPercentage,
                StartDateUtc = request.StartDateUtc,
                DueDateUtc = request.DueDateUtc,
                ProjectId = request.ProjectId,
                AssignedUserId = request.AssignedUserId,
                CreatedByUserId = GetCurrentUserId()
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetTaskById), new { taskId = response.Id }, response);
    }

    [HttpPut("{taskId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TaskDetailsDto>> UpdateTask(
        Guid taskId,
        [FromBody] UpdateTaskRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateTaskCommandHandler.HandleAsync(
            new UpdateTaskCommand
            {
                TaskId = taskId,
                Title = request.Title,
                Description = request.Description,
                StartDateUtc = request.StartDateUtc,
                DueDateUtc = request.DueDateUtc,
                ProjectId = request.ProjectId,
                UpdatedByUserId = GetCurrentUserId()
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{taskId:guid}/status")]
    [Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailsDto>> UpdateTaskStatus(
        Guid taskId,
        [FromBody] UpdateTaskStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateTaskStatusCommandHandler.HandleAsync(
            new UpdateTaskStatusCommand
            {
                TaskId = taskId,
                Status = request.Status,
                UpdatedByUserId = GetCurrentUserId()
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{taskId:guid}/priority")]
    [Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailsDto>> UpdateTaskPriority(
        Guid taskId,
        [FromBody] UpdateTaskPriorityRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateTaskPriorityCommandHandler.HandleAsync(
            new UpdateTaskPriorityCommand
            {
                TaskId = taskId,
                Priority = request.Priority,
                UpdatedByUserId = GetCurrentUserId()
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{taskId:guid}/progress")]
    [Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailsDto>> UpdateTaskProgress(
        Guid taskId,
        [FromBody] UpdateTaskProgressRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateTaskProgressCommandHandler.HandleAsync(
            new UpdateTaskProgressCommand
            {
                TaskId = taskId,
                ProgressPercentage = request.ProgressPercentage,
                UpdatedByUserId = GetCurrentUserId()
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{taskId:guid}/assignee")]
    [Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailsDto>> AssignTask(
        Guid taskId,
        [FromBody] AssignTaskRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await assignTaskCommandHandler.HandleAsync(
            new AssignTaskCommand
            {
                TaskId = taskId,
                AssignedUserId = request.AssignedUserId,
                UpdatedByUserId = GetCurrentUserId()
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpDelete("{taskId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.ProjectManagerOrAdmin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid taskId, CancellationToken cancellationToken)
    {
        await deleteTaskCommandHandler.HandleAsync(
            new DeleteTaskCommand
            {
                TaskId = taskId,
                DeletedByUserId = GetCurrentUserId()
            },
            cancellationToken);

        return NoContent();
    }

    [HttpGet("{taskId:guid}/comments")]
    [ProducesResponseType(typeof(IReadOnlyList<CommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetComments(Guid taskId, CancellationToken cancellationToken)
    {
        var response = await getCommentsByTaskQueryHandler.HandleAsync(
            new GetCommentsByTaskQuery { TaskId = taskId },
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("{taskId:guid}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> AddComment(
        Guid taskId,
        [FromBody] AddCommentRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await addCommentCommandHandler.HandleAsync(
            new AddCommentCommand
            {
                TaskId = taskId,
                Content = request.Content,
                UserId = GetCurrentUserId()
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetComments), new { taskId }, response);
    }

    [HttpPut("comments/{commentId:guid}")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> UpdateComment(
        Guid commentId,
        [FromBody] UpdateCommentRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateCommentCommandHandler.HandleAsync(
            new UpdateCommentCommand
            {
                CommentId = commentId,
                Content = request.Content,
                UserId = GetCurrentUserId(),
                CurrentUserRole = GetCurrentUserRole()
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpDelete("comments/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(Guid commentId, CancellationToken cancellationToken)
    {
        await deleteCommentCommandHandler.HandleAsync(
            new DeleteCommentCommand
            {
                CommentId = commentId,
                UserId = GetCurrentUserId(),
                CurrentUserRole = GetCurrentUserRole()
            },
            cancellationToken);

        return NoContent();
    }

    [HttpGet("{taskId:guid}/activity")]
    [ProducesResponseType(typeof(IReadOnlyList<ActivityLogDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<ActivityLogDto>>> GetTaskActivity(Guid taskId, CancellationToken cancellationToken)
    {
        var response = await getTaskActivityQueryHandler.HandleAsync(
            new GetTaskActivityQuery { TaskId = taskId },
            cancellationToken);

        return Ok(response);
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
    }
}