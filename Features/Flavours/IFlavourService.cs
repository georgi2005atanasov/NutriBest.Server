namespace NutriBest.Server.Features.Flavours
{
    public interface IFlavourService
    {
        Task<int> Create(string name);

        Task<bool> Remove(string name);
    }
}
