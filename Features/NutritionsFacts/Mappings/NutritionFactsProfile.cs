namespace NutriBest.Server.Features.NutritionsFacts.Mappings
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.NutritionsFacts.Models;

    public class NutritionFactsProfile : AutoMapper.Profile
    {
        public NutritionFactsProfile()
        {
            CreateMap<Product, NutritionFactsServiceModel>()
                .ForMember(dest => dest.Proteins, src => src.MapFrom(x => x.NutritionFacts.Proteins))
                .ForMember(dest => dest.SaturatedFats, src => src.MapFrom(x => x.NutritionFacts.SaturatedFats))
                .ForMember(dest => dest.Fats, src => src.MapFrom(x => x.NutritionFacts.Fats))
                .ForMember(dest => dest.Sugars, src => src.MapFrom(x => x.NutritionFacts.Sugars))
                .ForMember(dest => dest.Carbohydrates, src => src.MapFrom(x => x.NutritionFacts.Carbohydrates))
                .ForMember(dest => dest.Salt, src => src.MapFrom(x => x.NutritionFacts.Salt))
                .ForMember(dest => dest.EnergyValue, src => src.MapFrom(x => x.NutritionFacts.EnergyValue));
        }
    }
}
