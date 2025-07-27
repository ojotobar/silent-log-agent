using SilentLogAgent.Services;

namespace SilentLogAgent.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<ISpecLoader, SpecLoader>();
        }
    }
}
