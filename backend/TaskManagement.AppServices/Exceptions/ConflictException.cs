namespace TaskManagement.AppServices.Exceptions;

public sealed class ConflictException(string message) : Exception(message)
{
}