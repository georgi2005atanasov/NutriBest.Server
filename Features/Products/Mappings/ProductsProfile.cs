namespace NutriBest.Server.Features.Products.Mappings
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;
    using NutriBest.Server.Features.Products.Models;

    public class ProductsProfile : AutoMapper.Profile
    {
        public ProductsProfile()
        {
            CreateMap<Product, ProductServiceModel>()
                .ForMember(dest => dest.Categories, src => src.MapFrom(x => x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList()))
                .ForMember(dest => dest.Brand, src => src.MapFrom(x => x.Brand!.Name)) // be aware
                .ForMember(dest => dest.Price, src => src.MapFrom(x => x.StartingPrice))
                .ForMember(dest => dest.Image, src => src.MapFrom(x => new ImageListingServiceModel
                {
                    ImageData = x.ProductImage!.ImageData, //be aware
                    ContentType = x.ProductImage.ContentType
                }));
        }
    }
}
