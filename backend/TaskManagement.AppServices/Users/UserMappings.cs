using TaskManagement.Domain.Entities;
using TaskManagement.DTO.Models.User;

namespace TaskManagement.AppServices.Users;

public static class UserMappings
{
    public static UserListItemDto ToListItemDto(this User user)
    {
        return new UserListItemDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc
        };
    }

    public static UserDetailsDto ToDetailsDto(this User user)
    {
        return new UserDetailsDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAtUtc = user.CreatedAtUtc,
            UpdatedAtUtc = user.UpdatedAtUtc
        };
    }
}