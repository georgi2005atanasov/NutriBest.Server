namespace NutriBest.Server.Features.Promotions.Mappings
{
    using NutriBest.Server.Features.Promotions.Models;
    using AutoMapper;
    using NutriBest.Server.Data.Models;

    public class PromotionsProfile : AutoMapper.Profile
    {
        public PromotionsProfile()
        {
            CreateMap<Promotion, PromotionServiceModel>();
        }
    }
}
