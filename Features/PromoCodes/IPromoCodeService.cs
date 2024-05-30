namespace NutriBest.Server.Features.PromoCodes
{
    using NutriBest.Server.Features.PromoCodes.Models;

    public interface IPromoCodeService
    {
        Task<List<string>> Create(decimal discountPercentage,
            int count,
            string description);

        Task<PromoCodeListingModel> GetByCode(string code);

        Task<bool> DisableByCode(string code);

        Task<bool> DisableByDescription(string description);

        Task<(List<string>, int)> GetByDescription(string description);

        Task<List<PromoCodeByDescriptionServiceModel>> All();
    }
}
