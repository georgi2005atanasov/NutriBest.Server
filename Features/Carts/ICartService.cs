namespace NutriBest.Server.Features.Carts
{
    public interface ICartService
    {
        Task<int?> Add(int productId);

        Task<bool> Remove(int productId);

        Task<bool> Clean();
    }
}
