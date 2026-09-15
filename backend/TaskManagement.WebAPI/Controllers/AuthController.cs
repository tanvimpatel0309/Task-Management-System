using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.AppServices.Authentication;
using TaskManagement.Command.Auth.Login;
using TaskManagement.DTO.Models.Auth;

namespace TaskManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(ICommandHandler<LoginCommand, AuthenticationResponseDto> loginCommandHandler) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationResponseDto>> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var response = await loginCommandHandler.HandleAsync(
            new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            },
            cancellationToken);

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(AuthenticatedUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<AuthenticatedUserDto> Me()
    {
        return Ok(new AuthenticatedUserDto
        {
            Id = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString()),
            FirstName = User.FindFirstValue(JwtRegisteredClaimNames.GivenName) ?? string.Empty,
            LastName = User.FindFirstValue(JwtRegisteredClaimNames.FamilyName) ?? string.Empty,
            Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            Role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty
        });
    }

    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    [HttpGet("admin-check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult AdminCheck()
    {
        return Ok(new { message = "Admin access granted." });
    }
}