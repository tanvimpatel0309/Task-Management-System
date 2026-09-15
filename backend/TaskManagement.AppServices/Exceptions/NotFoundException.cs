namespace TaskManagement.AppServices.Exceptions;

public sealed class NotFoundException(string message) : Exception(message)
{
}