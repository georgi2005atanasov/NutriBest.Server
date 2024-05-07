namespace NutriBest.Server.Features.Brands
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using NutriBest.Server.Data;
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
            => await db.Brands
            .Select(x => new BrandServiceModel
            {
                Name = x.Name,
                BrandLogoId = x.BrandLogoId
            })
            .ToListAsync();

        //public Task<int> Create(string name, string? description, IFormFile image)
        //{
        //    var image
        //}

        public async Task<BrandDetailsServiceModel?> Get(string brandName)
            => await db.Brands
            .Where(x => x.Name == brandName)
            .Select(x => new BrandDetailsServiceModel
            {
                Name = x.Name,
                BrandLogoId = x.BrandLogoId,
                Description = x.Description
            })
            .FirstOrDefaultAsync();

        public Task<BrandServiceModel> Remove(string brandName)
        {
            throw new NotImplementedException();
        }

        Task<BrandServiceModel> IBrandService.Get(string brandName)
        {
            throw new NotImplementedException();
        }
    }
}
