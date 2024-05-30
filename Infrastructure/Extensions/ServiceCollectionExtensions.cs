namespace NutriBest.Server.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Admin;
    using NutriBest.Server.Features.Brands;
    using NutriBest.Server.Features.Carts;
    using NutriBest.Server.Features.Categories;
    using NutriBest.Server.Features.Flavours;
    using NutriBest.Server.Features.Identity;
    using NutriBest.Server.Features.Images;
    using NutriBest.Server.Features.NutritionsFacts;
    using NutriBest.Server.Features.OrderDetails;
    using NutriBest.Server.Features.Orders;
    using NutriBest.Server.Features.Packages;
    using NutriBest.Server.Features.Products;
    using NutriBest.Server.Features.ProductsDetails;
    using NutriBest.Server.Features.ProductsPromotions;
    using NutriBest.Server.Features.PromoCodes;
    using NutriBest.Server.Features.Promotions;
    using NutriBest.Server.Infrastructure.Filters;
    using NutriBest.Server.Infrastructure.Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddHostedService<PromoCodeCleanupService>()
                .AddHostedService<PromotionCleanupService>()
                .AddHostedService<PromotionActivationService>()
                .AddScoped<ICurrentUserService, CurrentUserService>()
                .AddTransient<IAdminService, AdminService>()
                .AddTransient<IIdentityService, IdentityService>()
                .AddTransient<IProfileService, ProfileService>()
                .AddTransient<IProductService, ProductService>()
                .AddTransient<IProductDetailsService, ProductDetailsService>()
                .AddTransient<INutritionFactsService, NutritionFactsService>()
                .AddTransient<IPromotionService, PromotionService>()
                .AddTransient<IProductPromotionService, ProductPromotionService>()
                .AddTransient<ICartService, CartService>()
                .AddTransient<IImageService, ImageService>()
                .AddTransient<ICategoryService, CategoryService>()
                .AddTransient<IBrandService, BrandService>()
                .AddTransient<IFlavourService, FlavourService>()
                .AddTransient<IPackageService, PackageService>()
                .AddTransient<IPromoCodeService, PromoCodeService>()
                .AddTransient<IOrderDetailsService, OrderDetailsService>()
                .AddTransient<IGuestOrderService, GuestOrderService>()
                .AddTransient<IOrderService, OrderService>();

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
             {
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequiredLength = 6;
                 options.Password.RequireDigit = false;
                 options.Password.RequireUppercase = false;
                 options.User.RequireUniqueEmail = true;
             })
            .AddEntityFrameworkStores<NutriBestDbContext>();

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
