namespace NutriBest.Server.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data.Extensions;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Data.Models.Base;
    using NutriBest.Server.Infrastructure.Services;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class NutriBestDbContext : IdentityDbContext<User>
    {
        private readonly ICurrentUserService currentUser;

        public DbSet<Product> Products { get; set; } = null!;

        public DbSet<ProductDetails> ProductsDetails { get; set; } = null!;

        public DbSet<Promotion> Promotions { get; set; } = null!;

        public DbSet<ProductImage> ProductsImages { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Brand> Brands { get; set; } = null!;

        public DbSet<BrandLogo> BrandsLogos { get; set; } = null!;

        public DbSet<ProductCategory> ProductsCategories { get; set; } = null!;

        public DbSet<Profile> Profiles { get; set; } = null!;

        public DbSet<Cart> Carts { get; set; } = null!;

        public DbSet<CartProduct> CartProducts { get; set; } = null!;

        public DbSet<UserOrder> UsersOrders { get; set; } = null!;

        public DbSet<GuestOrder> GuestsOrders { get; set; } = null!;

        public DbSet<Order> Orders { get; set; } = null!;

        public DbSet<OrderDetails> OrdersDetails { get; set; } = null!;

        public DbSet<Invoice> Invoices { get; set; } = null!;

        public DbSet<Country> Countries { get; set; } = null!;

        public DbSet<Address> Addresses { get; set; } = null!;

        public DbSet<City> Cities { get; set; } = null!;

        public DbSet<NutritionFacts> NutritionFacts { get; set; } = null!;

        public DbSet<Flavour> Flavours { get; set; } = null!;

        public DbSet<Package> Packages { get; set; } = null!;

        public DbSet<ProductPackageFlavour> ProductsPackagesFlavours { get; set; } = null!;

        public DbSet<PromoCode> PromoCodes { get; set; } = null!;

        public DbSet<ShippingDiscount> ShippingDiscounts { get; set; } = null!;

        public DbSet<Notification> Notifications { get; set; } = null!;

        public DbSet<Newsletter> Newsletter { get; set; } = null!;

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

            var modelBuilderType = typeof(ModelBuilderExtensions);
            var methods = modelBuilderType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                          .Where(m => m.Name.StartsWith("Configure") && m.GetParameters().Length == 1
                                                      && m.GetParameters()[0].ParameterType == typeof(ModelBuilder));

            foreach (var method in methods)
            {
                method.Invoke(null, new object[] { builder });
            }
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
