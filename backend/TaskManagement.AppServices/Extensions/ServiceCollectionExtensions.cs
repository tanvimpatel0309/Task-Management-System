using Microsoft.Extensions.DependencyInjection;

namespace TaskManagement.AppServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        return services;
    }
}