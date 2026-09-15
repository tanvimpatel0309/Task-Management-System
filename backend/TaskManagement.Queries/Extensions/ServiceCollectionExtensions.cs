using Microsoft.Extensions.DependencyInjection;
using TaskManagement.AppServices.Cqrs;
using TaskManagement.DTO.Models.Activity;
using TaskManagement.DTO.Models.Comment;
using TaskManagement.DTO.Models.Project;
using TaskManagement.DTO.Models.Task;
using TaskManagement.DTO.Models.User;
using TaskManagement.DTO.Utility;
using TaskManagement.Queries.Comment.GetCommentsByTask;
using TaskManagement.Queries.Project.GetProjectById;
using TaskManagement.Queries.Project.GetProjects;
using TaskManagement.Queries.Task.GetAssignableUsers;
using TaskManagement.Queries.Task.GetTaskActivity;
using TaskManagement.Queries.Task.GetTaskById;
using TaskManagement.Queries.Task.GetTasks;
using TaskManagement.Queries.User.GetUserById;
using TaskManagement.Queries.User.GetUsers;

namespace TaskManagement.Queries.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddQueryServices(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<GetProjectsQuery, IReadOnlyList<ProjectListItemDto>>, GetProjectsQueryHandler>();
        services.AddScoped<IQueryHandler<GetProjectByIdQuery, ProjectDetailsDto>, GetProjectByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetAssignableUsersQuery, IReadOnlyList<UserListItemDto>>, GetAssignableUsersQueryHandler>();
        services.AddScoped<IQueryHandler<GetTasksQuery, PagedResultDto<TaskListItemDto>>, GetTasksQueryHandler>();
        services.AddScoped<IQueryHandler<GetTaskByIdQuery, TaskDetailsDto>, GetTaskByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetTaskActivityQuery, IReadOnlyList<ActivityLogDto>>, GetTaskActivityQueryHandler>();
        services.AddScoped<IQueryHandler<GetCommentsByTaskQuery, IReadOnlyList<CommentDto>>, GetCommentsByTaskQueryHandler>();
        services.AddScoped<IQueryHandler<GetUsersQuery, IReadOnlyList<UserListItemDto>>, GetUsersQueryHandler>();
        services.AddScoped<IQueryHandler<GetUserByIdQuery, UserDetailsDto>, GetUserByIdQueryHandler>();

        return services;
    }
}