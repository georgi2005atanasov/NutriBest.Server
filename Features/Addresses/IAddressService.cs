namespace NutriBest.Server.Features.Addresses
{
    public interface IAddressService
    {
        Task<int> CreateGuestAddress(string street,
            int? streetNumber);
    }
}
