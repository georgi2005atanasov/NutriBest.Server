namespace NutriBest.Server.Infrastructure.Extensions
{
    using System.Text;

    public static class ConfigurationExtensions
    {
        public static IConfigurationSection GetAppSettings(this IConfiguration config)
            => config.GetSection("ApplicationSettings");
    }
}
