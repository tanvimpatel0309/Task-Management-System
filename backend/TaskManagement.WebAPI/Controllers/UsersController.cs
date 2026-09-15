using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Cqrs;
using TaskManagement.Command.User.ActivateUser;
using TaskManagement.Command.User.CreateUser;
using TaskManagement.Command.User.DeactivateUser;
using TaskManagement.Command.User.UpdateUser;
using TaskManagement.Command.User.UpdateUserRole;
using TaskManagement.DTO.Models.User;
using TaskManagement.Queries.User.GetUserById;
using TaskManagement.Queries.User.GetUsers;

namespace TaskManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
public sealed class UsersController(
    ICommandHandler<CreateUserCommand, UserDetailsDto> createUserCommandHandler,
    ICommandHandler<UpdateUserCommand, UserDetailsDto> updateUserCommandHandler,
    ICommandHandler<ActivateUserCommand, UserDetailsDto> activateUserCommandHandler,
    ICommandHandler<DeactivateUserCommand, UserDetailsDto> deactivateUserCommandHandler,
    ICommandHandler<UpdateUserRoleCommand, UserDetailsDto> updateUserRoleCommandHandler,
    IQueryHandler<GetUsersQuery, IReadOnlyList<UserListItemDto>> getUsersQueryHandler,
    IQueryHandler<GetUserByIdQuery, UserDetailsDto> getUserByIdQueryHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<UserListItemDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var response = await getUsersQueryHandler.HandleAsync(new GetUsersQuery(), cancellationToken);
        return Ok(response);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var response = await getUserByIdQueryHandler.HandleAsync(new GetUserByIdQuery { UserId = userId }, cancellationToken);
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDetailsDto>> CreateUser(
        [FromBody] CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await createUserCommandHandler.HandleAsync(
            new CreateUserCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                Role = request.Role,
                IsActive = request.IsActive
            },
            cancellationToken);

        return CreatedAtAction(nameof(GetUserById), new { userId = response.Id }, response);
    }

    [HttpPut("{userId:guid}")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDetailsDto>> UpdateUser(
        Guid userId,
        [FromBody] UpdateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateUserCommandHandler.HandleAsync(
            new UpdateUserCommand
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{userId:guid}/role")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> UpdateUserRole(
        Guid userId,
        [FromBody] UpdateUserRoleRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await updateUserRoleCommandHandler.HandleAsync(
            new UpdateUserRoleCommand
            {
                UserId = userId,
                Role = request.Role
            },
            cancellationToken);

        return Ok(response);
    }

    [HttpPatch("{userId:guid}/activate")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> ActivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var response = await activateUserCommandHandler.HandleAsync(new ActivateUserCommand { UserId = userId }, cancellationToken);
        return Ok(response);
    }

    [HttpPatch("{userId:guid}/deactivate")]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> DeactivateUser(Guid userId, CancellationToken cancellationToken)
    {
        var response = await deactivateUserCommandHandler.HandleAsync(new DeactivateUserCommand { UserId = userId }, cancellationToken);
        return Ok(response);
    }
}