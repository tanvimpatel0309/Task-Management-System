namespace TaskManagement.AppServices.Exceptions;

public sealed class UnauthorizedException(string message) : Exception(message)
{
}