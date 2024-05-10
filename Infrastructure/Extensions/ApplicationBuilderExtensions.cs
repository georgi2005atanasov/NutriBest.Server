﻿namespace NutriBest.Server.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Infrastructure.Middlewares;
    using System;

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
            SeedFlavours(dbContext);
            SeedPackages(dbContext);
            SeedEmployeeRole(services.ServiceProvider);
            SeedUserRole(services.ServiceProvider);
        }

        private static void SeedPackages(NutriBestDbContext db)
        {
            if (db.Packages != null && db.Packages.Any())
                return;

            Task.Run(async () =>
            {
                db.Packages!.Add(new Package { Grams = 100 });
                db.Packages.Add(new Package { Grams = 250 });
                db.Packages.Add(new Package { Grams = 500 });
                db.Packages.Add(new Package { Grams = 750 });
                db.Packages.Add(new Package { Grams = 1000 });
                db.Packages.Add(new Package { Grams = 1500 });
                db.Packages.Add(new Package { Grams = 2000 });
                db.Packages.Add(new Package { Grams = 2500 });

                await db.SaveChangesAsync();
            })
                .GetAwaiter()
                .GetResult();
        }

        private static void SeedFlavours(NutriBestDbContext db)
        {
            if (db.Flavours != null && db.Flavours.Any())
                return;

            Task.Run(async () =>
            {
                db.Flavours!.Add(new Flavour { FlavourName = "Chocolate" });
                db.Flavours.Add(new Flavour { FlavourName = "Vanilla" });
                db.Flavours.Add(new Flavour { FlavourName = "Strawberry" });
                db.Flavours.Add(new Flavour { FlavourName = "Banana" });
                db.Flavours.Add(new Flavour { FlavourName = "Cookies and Cream" });
                db.Flavours.Add(new Flavour { FlavourName = "Mint Chocolate" });
                db.Flavours.Add(new Flavour { FlavourName = "Cafe Latte" });
                db.Flavours.Add(new Flavour { FlavourName = "Salted Caramel" });
                db.Flavours.Add(new Flavour { FlavourName = "Lemon Lime" });
                db.Flavours.Add(new Flavour { FlavourName = "Blueberry" });
                db.Flavours.Add(new Flavour { FlavourName = "Mango" });
                db.Flavours.Add(new Flavour { FlavourName = "Peanut Butter" });
                db.Flavours.Add(new Flavour { FlavourName = "Cinnamon Roll" });
                db.Flavours.Add(new Flavour { FlavourName = "Matcha" });
                db.Flavours.Add(new Flavour { FlavourName = "Coconut" });

                await db.SaveChangesAsync();
            })
                .GetAwaiter()
                .GetResult();
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
