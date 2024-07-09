namespace NutriBest.Server.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationSection GetAppSettings(this IConfiguration config)
            => config.GetSection("ApplicationSettings");
    }
}
