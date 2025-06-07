namespace NutriBest.Server.Features.Brands
{
    using NutriBest.Server.Features.Brands.Models;

    public interface IBrandService
    {
        Task<IEnumerable<BrandServiceModel>> All();

        Task<bool> Remove(string brandName);

        Task<int> Create(string name, string? description, IFormFile? image);

        Task<List<BrandCountServiceModel>> GetProductsByBrandCount();
    }
}
