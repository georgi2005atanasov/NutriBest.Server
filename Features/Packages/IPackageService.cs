namespace NutriBest.Server.Features.Packages
{
    public interface IPackageService
    {
        Task<int> Create(int grams);

        Task<bool> Remove(int grams);
    }
}
