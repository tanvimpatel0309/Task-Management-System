using FluentValidation;
using Microsoft.Extensions.Options;
using TaskManagement.AppServices.Authentication;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.Domain.Contracts;
using TaskManagement.DTO.Models.Auth;

namespace TaskManagement.Command.Auth.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IUserPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IValidator<LoginCommand> validator,
    IOptions<JwtOptions> jwtOptions)
    : ICommandHandler<LoginCommand, AuthenticationResponseDto>
{
    public async Task<AuthenticationResponseDto> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !passwordHasher.VerifyPassword(user, command.Password))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedException("This user account is inactive.");
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenExpirationMinutes);
        var accessToken = jwtTokenGenerator.GenerateToken(user, expiresAtUtc);

        return new AuthenticationResponseDto
        {
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAtUtc,
            User = new AuthenticatedUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString()
            }
        };
    }
}