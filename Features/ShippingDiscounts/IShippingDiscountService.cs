namespace NutriBest.Server.Features.ShippingDiscounts
{
    using NutriBest.Server.Features.ShippingDiscounts.Models;

    public interface IShippingDiscountService
    {
        Task<AllShippingDiscountsServiceModel> All();

        Task<ShippingDiscountServiceModel> Get(string countryName);

        Task<int> Create(string countryName,
            decimal discountPercentage,
            DateTime? endDate,
            string description,
            string? minimumPrice);

        Task<bool> Remove(string country);
    }
}
