﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NutriBest.Server.Data.Models;

namespace NutriBest.Server.Data
{
    public class NutriBestDbContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductsImages { get; set; }

        public NutriBestDbContext(DbContextOptions<NutriBestDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>(e =>
            {
                e.HasOne(x => x.ProductImage)
                .WithOne(x => x.Product)
                .HasForeignKey<Product>(x => x.ProductImageId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}