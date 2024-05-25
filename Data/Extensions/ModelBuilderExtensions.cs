
namespace NutriBest.Server.Data.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
    using NutriBest.Server.Data.Enums;
    using NutriBest.Server.Data.Models;

    public static class ModelBuilderExtensions
    {
        public static void ConfigureProfile(this ModelBuilder builder)
        {

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

        }

        public static void ConfigureOrder(this ModelBuilder builder)
        {
            builder.Entity<Order>()
                .HasOne(x => x.Cart)
                .WithOne()
                .HasForeignKey<Order>(x => x.CartId);

            builder.Entity<Order>()
                .HasOne(x => x.OrderDetails)
                .WithOne(x => x.Order)
                .HasForeignKey<OrderDetails>(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public static void ConfigureProduct(this ModelBuilder builder)
        {
            builder.Entity<Product>(e =>
            {
                e.HasOne(x => x.ProductImage)
                .WithOne(x => x.Product)
                .HasForeignKey<Product>(x => x.ProductImageId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Product>(e =>
            {
                e.HasOne(x => x.Brand)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.BrandId)
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
                .HasOne(x => x.Promotion)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        public static void ConfigureUser(this ModelBuilder builder)
        {
            builder.Entity<User>(e =>
            {
                e.HasOne(x => x.Profile)
                .WithOne()
                .HasForeignKey<Profile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public static void ConfigureProductCategory(this ModelBuilder builder)
        {
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
        }

        public static void ConfigureProductImage(this ModelBuilder builder)
        {
            builder.Entity<ProductImage>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        public static void ConfigureBrand(this ModelBuilder builder)
        {
            builder.Entity<Brand>(e =>
            {
                e.HasOne(x => x.BrandLogo)
                .WithOne(x => x.Brand)
                .HasForeignKey<Brand>(x => x.BrandLogoId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Brand>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        public static void ConfigureProductPackageFlavour(this ModelBuilder builder)
        {
            builder.Entity<ProductPackageFlavour>(e =>
            {
                e.HasKey(e => new { e.ProductId, e.FlavourId, e.PackageId });

                e.HasOne(x => x.Product)
                .WithMany(x => x.ProductPackageFlavours)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Package)
                .WithMany(x => x.ProductPackageFlavours)
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Flavour)
                .WithMany(x => x.ProductPackageFlavours)
                .HasForeignKey(x => x.FlavourId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public static void ConfigureCartProduct(this ModelBuilder builder)
        {
            builder.Entity<CartProduct>()
                .HasOne(x => x.Cart)
                .WithMany(x => x.CartProducts)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public static void ConfigurePromotion(this ModelBuilder builder)
        {
            builder.Entity<Promotion>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        public static void ConfigureProductPackageFlavours(this ModelBuilder builder)
        {
            builder.Entity<ProductPackageFlavour>()
                .HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<Flavour>()
                .HasQueryFilter(x => !x.IsDeleted);

            builder.Entity<Package>()
                .HasQueryFilter(x => !x.IsDeleted);
        }

        public static void ConfigurePromoCodes(this ModelBuilder builder)
        {
            builder.Entity<PromoCode>(e =>
            {
                e.HasQueryFilter(x => x.IsValid && !x.IsDeleted);
            });
        }
    }
}
