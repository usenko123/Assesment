using Microsoft.Extensions.Configuration;

namespace Assessment.Api.Extensions;

public static class CorsExtensions
{
    public const string FrontendPolicy = "Frontend";
    private const string ConfigSection = "Cors:AllowedOrigins";

    public static IServiceCollection AddFrontendCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection(ConfigSection).Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendPolicy, policy =>
            {
                if (origins.Length == 0)
                {
                    policy.WithOrigins("http://localhost:4200");
                }
                else
                {
                    policy.WithOrigins(origins);
                }

                policy.AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }
}
