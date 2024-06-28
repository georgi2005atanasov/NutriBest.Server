namespace NutriBest.Server.Features.Profile.Mappings
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Profile.Models;

    public class ProfilesProfile : AutoMapper.Profile
    {
        public ProfilesProfile()
        {
            CreateMap<User, ProfileServiceModel>()
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Profile.Name))
                .ForMember(dest => dest.Age, src => src.MapFrom(x => x.Profile.Age))
                .ForMember(dest => dest.Gender, src => src.MapFrom(x => x.Profile.Gender.ToString()));
        }
    }
}
