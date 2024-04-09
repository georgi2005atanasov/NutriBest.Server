using System.Text;

namespace NutriBest.Server.Infrastructure
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationSection GetAppSettings(this IConfiguration config)
            => config.GetSection("ApplicationSettings");
    }
}
