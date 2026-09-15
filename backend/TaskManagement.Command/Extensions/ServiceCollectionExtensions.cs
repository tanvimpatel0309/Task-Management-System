using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.AppServices.Authentication;
using TaskManagement.Command.Comment.AddComment;
using TaskManagement.Command.Comment.DeleteComment;
using TaskManagement.Command.Comment.UpdateComment;
using TaskManagement.Command.Auth.Login;
using TaskManagement.Command.Project.CreateProject;
using TaskManagement.Command.Project.DeleteProject;
using TaskManagement.Command.Project.UpdateProject;
using TaskManagement.Command.Project.UpdateProjectStatus;
using TaskManagement.Command.Task.AssignTask;
using TaskManagement.Command.Task.CreateTask;
using TaskManagement.Command.Task.DeleteTask;
using TaskManagement.Command.Task.UpdateTask;
using TaskManagement.Command.Task.UpdateTaskPriority;
using TaskManagement.Command.Task.UpdateTaskProgress;
using TaskManagement.Command.Task.UpdateTaskStatus;
using TaskManagement.Command.User.ActivateUser;
using TaskManagement.Command.User.CreateUser;
using TaskManagement.Command.User.DeactivateUser;
using TaskManagement.Command.User.UpdateUser;
using TaskManagement.Command.User.UpdateUserRole;
using TaskManagement.DTO.Models.Auth;
using TaskManagement.DTO.Models.Comment;
using TaskManagement.DTO.Models.Project;
using TaskManagement.DTO.Models.Task;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.Command.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AssemblyReference>();
        services.AddScoped<ICommandHandler<LoginCommand, AuthenticationResponseDto>, LoginCommandHandler>();
        services.AddScoped<ICommandHandler<CreateProjectCommand, ProjectDetailsDto>, CreateProjectCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateProjectCommand, ProjectDetailsDto>, UpdateProjectCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteProjectCommand, bool>, DeleteProjectCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateProjectStatusCommand, ProjectDetailsDto>, UpdateProjectStatusCommandHandler>();
        services.AddScoped<ICommandHandler<CreateTaskCommand, TaskDetailsDto>, CreateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTaskCommand, TaskDetailsDto>, UpdateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTaskStatusCommand, TaskDetailsDto>, UpdateTaskStatusCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTaskPriorityCommand, TaskDetailsDto>, UpdateTaskPriorityCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTaskProgressCommand, TaskDetailsDto>, UpdateTaskProgressCommandHandler>();
        services.AddScoped<ICommandHandler<AssignTaskCommand, TaskDetailsDto>, AssignTaskCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTaskCommand, bool>, DeleteTaskCommandHandler>();
        services.AddScoped<ICommandHandler<AddCommentCommand, CommentDto>, AddCommentCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateCommentCommand, CommentDto>, UpdateCommentCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteCommentCommand, bool>, DeleteCommentCommandHandler>();
        services.AddScoped<ICommandHandler<CreateUserCommand, UserDetailsDto>, CreateUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserCommand, UserDetailsDto>, UpdateUserCommandHandler>();
        services.AddScoped<ICommandHandler<ActivateUserCommand, UserDetailsDto>, ActivateUserCommandHandler>();
        services.AddScoped<ICommandHandler<DeactivateUserCommand, UserDetailsDto>, DeactivateUserCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateUserRoleCommand, UserDetailsDto>, UpdateUserRoleCommandHandler>();

        return services;
    }
}