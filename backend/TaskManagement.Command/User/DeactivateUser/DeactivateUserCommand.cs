namespace TaskManagement.Command.User.DeactivateUser;

public sealed class DeactivateUserCommand
{
    public Guid UserId { get; init; }
}