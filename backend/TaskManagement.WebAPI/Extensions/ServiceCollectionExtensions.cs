using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TaskManagement.AppServices.Authentication;
using TaskManagement.DTO.Utility;
using TaskManagement.WebAPI.Middleware;

namespace TaskManagement.WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    private const string FrontendCorsPolicy = "FrontendCors";

    public static IServiceCollection AddWebApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddCors(options =>
        {
            options.AddPolicy(
                FrontendCorsPolicy,
                policy => policy
                    .WithOrigins("http://localhost:5173", "http://localhost:4173")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Task Management API",
                Version = "v1"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(
                options =>
                    !string.IsNullOrWhiteSpace(options.Issuer)
                    && !string.IsNullOrWhiteSpace(options.Audience)
                    && !string.IsNullOrWhiteSpace(options.SigningKey)
                    && options.AccessTokenExpirationMinutes > 0,
                "JWT configuration is invalid. Supply Jwt:Issuer, Jwt:Audience, Jwt:SigningKey, and a positive Jwt:AccessTokenExpirationMinutes value.")
            .ValidateOnStart();

        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new ApiErrorResponse("Authentication is required."));
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new ApiErrorResponse("You do not have permission to access this resource."));
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.AdminOnly,
                policy => policy.RequireRole(TaskManagement.Domain.Enums.UserRole.Admin.ToString()));

            options.AddPolicy(
                AuthorizationPolicies.ProjectManagerOrAdmin,
                policy => policy.RequireRole(
                    TaskManagement.Domain.Enums.UserRole.Admin.ToString(),
                    TaskManagement.Domain.Enums.UserRole.ProjectManager.ToString()));

            options.AddPolicy(
                AuthorizationPolicies.AnyTeamMember,
                policy => policy.RequireRole(
                    TaskManagement.Domain.Enums.UserRole.Admin.ToString(),
                    TaskManagement.Domain.Enums.UserRole.ProjectManager.ToString(),
                    TaskManagement.Domain.Enums.UserRole.TeamMember.ToString()));
        });

        return services;
    }

    public static IApplicationBuilder UseWebApiPipeline(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(FrontendCorsPolicy);

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}