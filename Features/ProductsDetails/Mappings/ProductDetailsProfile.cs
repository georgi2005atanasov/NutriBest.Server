﻿namespace NutriBest.Server.Features.ProductsDetails.Mappings
{
    using NutriBest.Server.Data.Models;
    using NutriBest.Server.Features.Images.Models;
    using NutriBest.Server.Features.ProductsDetails.Models;

    public class ProductDetailsProfile : AutoMapper.Profile
    {
        public ProductDetailsProfile()
        {
            CreateMap<Product, ProductDetailsServiceModel>()
                .ForMember(dest => dest.Image, src => src
                          .MapFrom(x => new ImageListingServiceModel
                          {
                              ImageData = x.ProductImage.ImageData,
                              ContentType = x.ProductImage.ContentType
                          }))
                .ForMember(dest => dest.Categories, src => src
                          .MapFrom(x => x.ProductsCategories
                          .Select(c => c.Category.Name)
                          .ToList()))
                .ForMember(dest => dest.HowToUse, src => src.MapFrom(x => x.ProductDetails.HowToUse))
                .ForMember(dest => dest.ServingsPerContainer, src => src.MapFrom(x => x.ProductDetails.ServingsPerContainer))
                .ForMember(dest => dest.ServingSize, src => src.MapFrom(x => x.ProductDetails.ServingSize));
        }
    }
}
