using NutriBest.Server.Data.Models;
using NutriBest.Server.Features.Images.Models;
using NutriBest.Server.Features.Products.Models;

namespace NutriBest.Server.Features.Products.Mappings
{
    public class ProductsProfile : AutoMapper.Profile
    {
        public ProductsProfile()
        {
            CreateMap<Product, ProductServiceModel>()
                .ForMember(dest => dest.Categories, src => src.MapFrom(x => x.ProductsCategories
                             .Select(c => c.Category.Name)
                             .ToList()))
                .ForMember(dest => dest.Brand, src => src.MapFrom(x => x.Brand!.Name)) // be aware
                .ForMember(dest => dest.Image, src => src.MapFrom(x => new ImageListingServiceModel
                 {
                     ImageData = x.ProductImage.ImageData,
                     ContentType = x.ProductImage.ContentType
                 }));
        }
    }
}
