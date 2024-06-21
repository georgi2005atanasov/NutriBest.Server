namespace NutriBest.Server.Features.Packages
{
    using NutriBest.Server.Features.Packages.Models;

    public interface IPackageService
    {
        Task<List<PackageServiceModel>> All();

        Task<int> Create(int grams);

        Task<bool> Remove(int grams);

        Task<List<PackageCountServiceModel>> GetProductsCountByQuantity();
    }
}
