namespace TaskManagement.AppServices.Authentication;

public static class AuthorizationPolicies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string ProjectManagerOrAdmin = nameof(ProjectManagerOrAdmin);
    public const string AnyTeamMember = nameof(AnyTeamMember);
}