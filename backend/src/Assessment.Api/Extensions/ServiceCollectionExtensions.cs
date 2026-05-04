using Assessment.Api.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Assessment.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddFrontendCors(configuration);
        return services;
    }
}
