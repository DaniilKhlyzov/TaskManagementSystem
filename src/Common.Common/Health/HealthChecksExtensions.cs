using Microsoft.Extensions.DependencyInjection;

namespace Common.Common.Health
{
    public static class HealthChecksExtensions
    {
        public static IServiceCollection AddCommonHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks();
            return services;
        }
    }
}
