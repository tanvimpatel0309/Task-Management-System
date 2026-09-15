using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TaskManagement.AppServices.Authentication;

namespace TaskManagement.AppServices.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<DevelopmentSeedOptions>()
            .Bind(configuration.GetSection(DevelopmentSeedOptions.SectionName));

        return services;
    }
}