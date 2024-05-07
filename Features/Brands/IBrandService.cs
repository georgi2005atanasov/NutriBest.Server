namespace NutriBest.Server.Features.Brands
{
    using NutriBest.Server.Features.Brands.Models;

    public interface IBrandService
    {
        Task<IEnumerable<BrandServiceModel>> All();

        Task<BrandServiceModel> Get(string brandName);

        Task<BrandServiceModel> Remove(string brandName);

        //Task<int> Create(string name, string? description, IFormFile image);
        //Task<BrandServiceModel> Update(string brandName); gotta think more on update
    }
}
