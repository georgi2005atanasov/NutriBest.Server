namespace NutriBest.Server.Infrastructure.Extensions
{
    using System.Reflection;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Infrastructure.Filters;
    using NutriBest.Server.Infrastructure.Services;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddHostedService<PromoCodeCleanupService>()
                .AddHostedService<PromotionCleanupService>()
                .AddHostedService<PromotionActivationService>()
                .AddHostedService<ShippingDiscountCleanupService>();

            var assembly = Assembly.GetExecutingAssembly(); 

            RegisterServices(services, assembly, typeof(IScopedService), ServiceLifetime.Scoped);
            RegisterServices(services, assembly, typeof(ITransientService), ServiceLifetime.Transient);

            return services;
        }

        private static void RegisterServices(IServiceCollection services, Assembly assembly, Type interfaceType, ServiceLifetime lifetime)
        {
            var types = assembly.GetTypes()
                                .Where(t => t.GetInterfaces().Any(i => i == interfaceType) && !t.IsAbstract && !t.IsInterface);

            foreach (var implementationType in types)
            {
                var serviceType = implementationType.GetInterfaces().LastOrDefault(i => i != interfaceType);
                if (serviceType != null)
                {
                    Console.WriteLine($"Registering service: {serviceType.Name} implemented by {implementationType.Name} as {lifetime}");
                    services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
                }
            }
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
             {
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireLowercase = true;
                 options.Password.RequiredLength = 9;
                 options.Password.RequireDigit = true;
                 options.Password.RequireUppercase = true;
                 options.User.RequireUniqueEmail = true;
             })
            .AddEntityFrameworkStores<NutriBestDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
            byte[] secret)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secret),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<NutriBestDbContext>
                    (options => options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ModelOrNotFoundActionFilter>();
            });

            return services;
        }
    }
}
