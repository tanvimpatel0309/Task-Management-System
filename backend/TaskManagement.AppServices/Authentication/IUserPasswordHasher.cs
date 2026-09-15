using TaskManagement.Domain.Entities;

namespace TaskManagement.AppServices.Authentication;

public interface IUserPasswordHasher
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string password);
}