namespace TaskManagement.Command.User.UpdateUserRole;

public sealed class UpdateUserRoleCommand
{
    public Guid UserId { get; init; }
    public string Role { get; init; } = string.Empty;
}