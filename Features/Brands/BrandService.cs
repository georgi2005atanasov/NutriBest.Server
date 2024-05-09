namespace NutriBest.Server.Features.Brands
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Brands.Models;
    using NutriBest.Server.Features.Images;

    public class BrandService : IBrandService
    {
        private readonly NutriBestDbContext db;
        private readonly IImageService imageService;

        public BrandService(NutriBestDbContext db,
            IImageService imageService)
        {
            this.db = db;
            this.imageService = imageService;
        }

        public async Task<IEnumerable<BrandServiceModel>> All()
        {
            var brands = await db.Brands
            .Select(x => new BrandServiceModel
            {
                Id = x.Id,
                Name = x.Name,
                BrandLogoId = x.BrandLogoId,
                Description = x.Description
            })
            .ToListAsync();

            foreach (var brand in brands)
            {
                if (brand.BrandLogoId != null)
                {
                    brand.BrandLogo = await imageService.GetImageByBrandId(brand.Name);
                }
            }

            return brands;
        }

        public async Task<int> Create(string name, string? description, IFormFile? image)
        {
            var brandLogo = new BrandLogo();

            if (image != null)
            {
                brandLogo = await imageService
                    .CreateImage<BrandLogo>(image, image.ContentType);
            }

            db.BrandsLogos.Add(brandLogo);

            var brand = new Brand
            {
                Name = name,
                Description = description,
                BrandLogo = brandLogo
            };

            db.Brands.Add(brand);

            await db.SaveChangesAsync();

            return brand.Id;
        }

        public async Task<BrandDetailsServiceModel?> Get(string brandName)
            => await db.Brands
            .Where(x => x.Name == brandName)
            .Select(x => new BrandDetailsServiceModel
            {
                Name = x.Name,
                BrandLogoId = x.BrandLogoId
            })
            .FirstOrDefaultAsync();

        // The deletion of a brand will delete the products
        // with this brand and also the promotions
        // gotta be aware of the fact that i might have problems in here
        public async Task<bool> Remove(string brandName)
        {
            var brand = await db.Brands
                .FirstOrDefaultAsync(x => x.Name == brandName);

            if (brand == null)
                throw new ArgumentNullException("Invalid brand!");

            var brandLogo = await db.BrandsLogos
                .FirstOrDefaultAsync(x => x.BrandLogoId == brand.BrandLogoId);

            var productsToDelete = db.Products
                .Where(x => x.BrandId == brand.Id)
                .AsQueryable();

            var promotionsToDelete = db.Promotions
                .Where(x => x.Brand == brand.Name)
                .AsQueryable();

            if (brandLogo != null)
                db.BrandsLogos.Remove(brandLogo);

            db.Products.RemoveRange(productsToDelete);

            db.Promotions.RemoveRange(promotionsToDelete);

            db.Brands.Remove(brand);

            await db.SaveChangesAsync();

            return true;
        }
    }
}
