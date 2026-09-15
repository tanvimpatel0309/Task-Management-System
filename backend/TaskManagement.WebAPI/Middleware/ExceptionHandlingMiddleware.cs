using System.Text.Json;
using FluentValidation;
using TaskManagement.AppServices.Exceptions;
using TaskManagement.DTO.Utility;

namespace TaskManagement.WebAPI.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status400BadRequest,
                new ApiErrorResponse(
                    "Validation failed.",
                    exception.Errors
                        .GroupBy(error => error.PropertyName)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(error => error.ErrorMessage).Distinct().ToArray())));
        }
        catch (UnauthorizedException exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status401Unauthorized,
                new ApiErrorResponse(exception.Message));
        }
        catch (NotFoundException exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status404NotFound,
                new ApiErrorResponse(exception.Message));
        }
        catch (ConflictException exception)
        {
            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status409Conflict,
                new ApiErrorResponse(exception.Message));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing request.");

            await WriteErrorResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                new ApiErrorResponse("An unexpected error occurred."));
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, ApiErrorResponse response)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}