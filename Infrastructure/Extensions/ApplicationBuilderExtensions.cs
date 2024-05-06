namespace NutriBest.Server.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Infrastructure.Middlewares;
    using System;
    using System.Security.Claims;

    public static class ApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var services = app.ApplicationServices.CreateScope();

            var dbContext = services.ServiceProvider.GetService<NutriBestDbContext>()!;

            dbContext.Database.Migrate();

            SeedAdministrator(services.ServiceProvider);
            SeedCategories(dbContext);
            SeedBrands(dbContext);
            SeedEmployeeRole(services.ServiceProvider);
            SeedUserRole(services.ServiceProvider);
        }

        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();

            string adminEmail = config.GetValue<string>("Admin:UserName");
            string adminPassword = config.GetValue<string>("Admin:Password");

            return builder.UseMiddleware<SwaggerBasicAuthMiddleware>(adminEmail, adminPassword);
        }

        private static void SeedUserRole(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            Task.Run(async () =>
            {
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    var role = new IdentityRole
                    {
                        Name = "User"
                    };

                    await roleManager.CreateAsync(role);
                }
            })
                .GetAwaiter()
                .GetResult();
        }

        private static void SeedEmployeeRole(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            Task.Run(async () =>
            {
                if (await roleManager.RoleExistsAsync("Employee"))
                    return;

                var role = new IdentityRole
                {
                    Name = "Employee"
                };

                await roleManager.CreateAsync(role);
            })
                .GetAwaiter()
                .GetResult();
        }

        private static void SeedCategories(NutriBestDbContext db)
        {
            if (db.Categories != null && db.Categories.Any())
                return;

            Task.Run(async () =>
            {
                await db.Categories!.AddRangeAsync(new List<Category>
                {
                    new Category{ Name="Proteins" },
                    new Category{ Name="Amino Acids" },
                    new Category{ Name="Pre-Workout" },
                    new Category{ Name="Vitamins" },
                    new Category{ Name="Creatines" },
                    new Category{ Name="Fat Burners" },
                    new Category{ Name="Mass Gainers" },
                    new Category{ Name="Post-Workout" },
                    new Category{ Name="Vegan" },
                    new Category{ Name="Recovery" },
                    new Category{ Name="Fish Oils" },
                    new Category{ Name="Promotions" }
                });

                await db.SaveChangesAsync();
            })
                .GetAwaiter()
                .GetResult();
        }

        private static void SeedBrands(NutriBestDbContext db)
        {
            if (db.Brands != null && db.Brands.Any())
                return;

            Task.Run(async () =>
            {
                await db.Brands!.AddRangeAsync(new List<Brand>
                {
                    new Brand{ Name="Nordic Naturals" },
                    new Brand{ Name="Garden of Life" },
                    new Brand{ Name="Klean Athlete" },
                    new Brand{ Name="Nature Made" },
                    new Brand{ Name="Optimim Nutrition" },
                    new Brand{ Name="Musle Tech" },
                });

                await db.SaveChangesAsync();
            })
                .GetAwaiter()
                .GetResult();
        }


        private static void SeedAdministrator(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var config = serviceProvider.GetService<IConfiguration>();

            Task.Run(async () =>
            {
                if (await roleManager.RoleExistsAsync("Administrator"))
                    return;

                var role = new IdentityRole { Name = "Administrator" };

                await roleManager.CreateAsync(role);

                string adminEmail = config.GetValue<string>("Admin:UserName");
                string adminPassword = config.GetValue<string>("Admin:Password");

                var user = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail
                };

                await userManager.CreateAsync(user, adminPassword);

                await userManager.AddToRoleAsync(user, role.Name);
            })
                .GetAwaiter()
                .GetResult();
        }
    }
}
