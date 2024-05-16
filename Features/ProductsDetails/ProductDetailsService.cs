namespace NutriBest.Server.Features.ProductsDetails
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Features.ProductsDetails.Models;

    public class ProductDetailsService : IProductDetailsService
    {
        private readonly NutriBestDbContext db;
        private readonly IMapper mapper;

        public ProductDetailsService(NutriBestDbContext db,
            IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ProductDetailsServiceModel> GetById(int id)
        {
            var product = await db.Products
                         .Include(x => x.ProductDetails)
                         .ProjectTo<ProductDetailsServiceModel>(mapper.ConfigurationProvider)
                         .FirstOrDefaultAsync(x => x.ProductId == id);

            if (product == null)
                throw new ArgumentNullException("Invalid product!");

            return product;
        }

        public async Task AddDetails(int productId, string? howToUse, string? servingSize)
        {
            var details = await db.ProductsDetails
                .FirstAsync(x => x.ProductId == productId);

            if (!string.IsNullOrEmpty(howToUse))
                details.HowToUse = howToUse;

            if (!string.IsNullOrEmpty(servingSize))
                details.ServingSize = servingSize;

            await db.SaveChangesAsync();
        }

        public async Task RemoveDetails(int productId)
        {
            var product = await db.Products
                .FirstAsync(x => x.ProductId == productId);

            var details = await db.ProductsDetails
                .FirstAsync(x => x.ProductId == productId);

            details.HowToUse = "";
            details.ServingSize = "";

            await db.SaveChangesAsync();
        }
    }
}
