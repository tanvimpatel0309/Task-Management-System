using TaskManagement.Domain.Entities;

namespace TaskManagement.AppServices.Authentication;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, DateTime expiresAtUtc);
}