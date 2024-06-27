namespace NutriBest.Server.Features.Promotions.Mappings
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Promotions.Models;

    public class PromotionsProfile : AutoMapper.Profile
    {
        public PromotionsProfile()
        {
            CreateMap<Promotion, PromotionServiceModel>();
        }
    }
}
