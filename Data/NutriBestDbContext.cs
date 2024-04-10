namespace NutriBest.Server.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data.Models;

    public class NutriBestDbContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductsImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductsCategories { get; set; }

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

            builder.Entity<ProductCategory>(e =>
            {
                e.HasKey(bc => new { bc.ProductId, bc.CategoryId });

                e.HasOne(bc => bc.Category)
                .WithMany(c => c.ProductsCategories)
                .HasForeignKey(bc => bc.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(bc => bc.Product)
                .WithMany(c => c.ProductsCategories)
                .HasForeignKey(bc => bc.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}