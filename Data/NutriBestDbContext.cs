namespace NutriBest.Server.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Data.Models.Base;
    using NutriBest.Server.Infrastructure.Services;
    using System.Threading;
    using System.Threading.Tasks;

    public class NutriBestDbContext : IdentityDbContext<User>
    {
        private readonly ICurrentUserService currentUser;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<ProductDetails> ProductsDetails { get; set; } = null!;

        public DbSet<ProductReview> ProductsReviews { get; set; } = null!;

        public DbSet<Promotion> Promotions { get; set; } = null!;

        public DbSet<ProductPromotion> ProductsPromotions { get; set; } = null!;

        public DbSet<ProductImage> ProductsImages { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<ProductCategory> ProductsCategories { get; set; } = null!;

        public DbSet<Profile> Profiles { get; set; } = null!;

        public DbSet<Cart> Carts { get; set; } = null!;

        public DbSet<CartProduct> CartProducts { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<OrderDetails> OrdersDetails { get; set; } = null!;

        public DbSet<Address> Addresses { get; set; } = null!;

        public DbSet<NutritionFacts> NutritionFacts { get; set; } = null!;

        public NutriBestDbContext(DbContextOptions<NutriBestDbContext> options,
            ICurrentUserService currentUser)
            : base(options)
        {
            this.currentUser = currentUser;
        }

        //be aware
        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        //be aware
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var genderConverter = new EnumToStringConverter<Gender>();
            var paymentMethodConverter = new EnumToStringConverter<PaymentMethod>();

            builder.Entity<Profile>()
               .HasOne(x => x.Cart)
               .WithOne(x => x.Profile)
               .HasForeignKey<Profile>(x => x.CartId)
               .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Profile>()
                .HasMany(x => x.Orders)
                .WithOne(x => x.Profile)
                .HasForeignKey(x => x.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Profile>()
                .Property(e => e.Gender)
                .HasConversion(genderConverter);

            builder.Entity<Profile>()
                .HasOne(x => x.Address)
                .WithOne()
                .HasForeignKey<Address>(x => x.ProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Order>()
                .HasOne(x => x.Cart)
                .WithOne()
                .HasForeignKey<Order>(x => x.CartId);

            builder.Entity<Order>()
                .HasOne(x => x.OrderDetails)
                .WithOne(x => x.Order)
                .HasForeignKey<OrderDetails>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CartProduct>()
                .HasOne(x => x.Cart)
                .WithMany(x => x.CartProducts)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<User>(e =>
            {
                e.HasOne(x => x.Profile)
                .WithOne()
                .HasForeignKey<Profile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Product>(e =>
            {
                e.HasOne(x => x.ProductImage)
                .WithOne(x => x.Product)
                .HasForeignKey<Product>(x => x.ProductImageId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Product>()
                .HasOne(x => x.ProductDetails)
                .WithOne()
                .HasForeignKey<ProductDetails>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(x => x.NutritionFacts)
                .WithOne(x => x.Product)
                .HasForeignKey<NutritionFacts>(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasMany(x => x.ProductReviews)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasMany(x => x.ProductPromotions)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<ProductImage>()
                .HasQueryFilter(x => !x.IsDeleted);

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

            builder.Entity<ProductCategory>()
                .HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<Promotion>()
                .HasMany(x => x.ProductPromotions)
                .WithOne(x => x.Promotion)
                .HasForeignKey(x => x.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Promotion>()
                .HasQueryFilter(x => !x.IsDeleted && x.IsActive);

            builder.Entity<ProductPromotion>()
                .HasKey(x => new { x.ProductId, x.PromotionId });

            builder.Entity<ProductPromotion>()
                .HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<ProductDetails>()
                .HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<NutritionFacts>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        private void ApplyAuditInformation()
            => this.ChangeTracker
                .Entries()
                .ToList()
                .ForEach(entry =>
                {
                    var user = currentUser.GetUserName();

                    if (entry.Entity is IDeletableEntity deletableEntity)
                    {
                        if (entry.State == EntityState.Deleted)
                        {
                            deletableEntity.DeletedOn = DateTime.UtcNow;
                            deletableEntity.IsDeleted = true;
                            deletableEntity.DeletedBy = user;

                            entry.State = EntityState.Modified;
                            return;
                        }
                    }

                    if (entry.Entity is IEntity entity)
                    {
                        if (entry.State == EntityState.Added)
                        {
                            entity.CreatedOn = DateTime.UtcNow;
                            entity.CreatedBy = user;
                        }
                        else if (entry.State == EntityState.Modified)
                        {
                            entity.ModifiedOn = DateTime.UtcNow;
                            entity.ModifiedBy = user;
                        }
                    }
                });
    }
}
