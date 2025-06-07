using NutriBest.Server.Utilities.Messages;

namespace NutriBest.Server.Features.Brands
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images;
    using NutriBest.Server.Features.Brands.Models;
    using NutriBest.Server.Infrastructure.Extensions.ServicesInterfaces;
    using static ErrorMessages.BrandsController;

    public class BrandService : IBrandService, ITransientService
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
            .OrderBy(x => x.Name)
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
            if (await db.Brands.AnyAsync(x => x.Name == name))
                throw new InvalidOperationException(BrandAlreadyExists);

            var brand = new Brand
            {
                Name = name,
                Description = description
            };

            var brandLogo = new BrandLogo();

            if (image != null)
            {
                brandLogo = await imageService
                    .CreateImage<BrandLogo>(image, image.ContentType);

                db.BrandsLogos.Add(brandLogo);
                brand.BrandLogo = brandLogo;
            }


            db.Brands.Add(brand);

            await db.SaveChangesAsync();

            return brand.Id;
        }

        // The deletion of a brand will delete the products
        // with this brand and also the promotions
        // gotta be aware of the fact that i might have problems in here
        public async Task<bool> Remove(string brandName)
        {
            var brand = await db.Brands
                .FirstOrDefaultAsync(x => x.Name == brandName);

            if (brand == null)
                throw new ArgumentNullException(InvalidBrandName);

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

        public async Task<List<BrandCountServiceModel>> GetProductsByBrandCount()
        {
            var result = await db.Brands
                .Select(x => new BrandCountServiceModel
                {
                    Name = x.Name,
                    Count = db.Products
                    .Where(y => y.BrandId == x.Id)
                    .Count()
                })
                .OrderBy(x => x.Name)
                .ToListAsync();

            return result;
        }
    }
}
