namespace NutriBest.Server.Features.PromoCodes
{
    public interface IPromoCodeService
    {
        Task<List<string>> Create(decimal? discountAmount,
            decimal? discountPercentage,
            int count);
    }
}
