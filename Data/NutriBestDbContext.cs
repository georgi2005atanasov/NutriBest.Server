namespace NutriBest.Server.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Data.Models.Base;
    using System.Threading;
    using System.Threading.Tasks;

    public class NutriBestDbContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductsImages { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<ProductCategory> ProductsCategories { get; set; } = null!;

        public NutriBestDbContext(DbContextOptions<NutriBestDbContext> options)
            : base(options)
        {
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            foreach (var entry in this.ChangeTracker.Entries())
            {
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            foreach (var entry in this.ChangeTracker.Entries())
            {
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}");
            }
            return base.SaveChangesAsync(cancellationToken);
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

        private void ApplyAuditInformation()
            => this.ChangeTracker
                .Entries()
                .ToList()
                .ForEach(entry =>
                {
                    if (entry.Entity is IDeletableEntity deletableEntity)
                    {
                        if (entry.State == EntityState.Deleted)
                        {
                            deletableEntity.DeletedOn = DateTime.UtcNow;
                            deletableEntity.IsDeleted = true;
                            // by who

                            entry.State = EntityState.Modified;
                            return;
                        }
                    }

                    if (entry.Entity is IEntity entity)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedOn = DateTime.UtcNow;
                            // by who
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entity.ModifiedOn = DateTime.UtcNow;
                            // by who
                        }
                    }
                });
    }
}
