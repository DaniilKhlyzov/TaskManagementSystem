using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Расширения DI для API Gateway
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPolly(this IServiceCollection services)
    {
        return services;
    }
}