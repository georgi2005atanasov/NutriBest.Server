namespace NutriBest.Server.Features.ProductsDetails.Mappings
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;
    using NutriBest.Server.Features.ProductsDetails.Models;

    public class ProductDetailsProfile : AutoMapper.Profile
    {
        public ProductDetailsProfile()
        {
            CreateMap<Product, ProductDetailsServiceModel>()
                .ForMember(dest => dest.Brand, src => src.MapFrom(x => x.Brand!.Name))
                .ForMember(dest => dest.Image, src => src
                          .MapFrom(x => new ImageListingServiceModel
                          {
                              ImageData = x.ProductImage!.ImageData, // be aware
                              ContentType = x.ProductImage.ContentType
                          }))
                .ForMember(dest => dest.Categories, src => src
                          .MapFrom(x => x.ProductsCategories
                          .Select(c => c.Category.Name)
                          .ToList()))
                .ForMember(dest => dest.HowToUse, src => src.MapFrom(x => x.ProductDetails.HowToUse))
                .ForMember(dest => dest.ServingSize, src => src.MapFrom(x => x.ProductDetails.ServingSize))
                .ForMember(dest => dest.Ingredients, src => src.MapFrom(x => x.ProductDetails.Ingredients))
                .ForMember(dest => dest.WhyChoose, src => src.MapFrom(x => x.ProductDetails.WhyChoose))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.Price, src => src.MapFrom(x => x.StartingPrice));
        }
    }
}
