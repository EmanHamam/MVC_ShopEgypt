using System;
using System.Linq;
using Mapster;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.DTOs.ReviewDtos;
using ShopEgypt.Domain.Entities;

namespace ShopEgypt.Application.Mappings
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            //same names so no need to configure
            TypeAdapterConfig<ProductImage, ProductImageDto>.NewConfig();

            TypeAdapterConfig<Product, ProductListItemDto>.NewConfig()
                .Map(
                    dest => dest.ThumbnailUrl,
                    src => src.ProductImages
                        .OrderBy(i => i.DisplayOrder)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault()
                )
                .Map(
                    dest => dest.AverageRating,
                    src => src.Reviews.Any()
                        ? src.Reviews.Average(r => r.Rating)
                        : 0
                )
                .Map(dest => dest.ReviewCount, src => src.Reviews.Count)
                .Map(dest => dest.IsOutOfStock, src => src.Stock <= 0);

            TypeAdapterConfig<Product, ProductDetailDto>.NewConfig()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : null)
                .Map(
                    dest => dest.AverageRating,
                    src => src.Reviews.Any()
                        ? src.Reviews.Average(r => r.Rating)
                        : 0
                )
                .Map(dest => dest.ReviewCount, src => src.Reviews.Count)
                .Map(dest => dest.Images, src => src.ProductImages
                    .OrderBy(i => i.DisplayOrder)
                    .ToList());


            TypeAdapterConfig<Review, ReviewDto>
                .NewConfig()
                .Map(dest => dest.ProductName, src => src.Product.Title)
                .Map(dest => dest.ApplicationUserId, src => "267824f8-683c-4812-afc8-1e1dbdafe519")  // testing 
                .Map(dest => dest.UserName, src => "mega03326@gmail.com")  // testing
                //.Map(dest => dest.UserName, src => src.ApplicationUser.UserName);
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow);


            // Create ReviewDto Mapping, no need to mapping as share the same properties
            

            TypeAdapterConfig<Category, CategoryDto>.NewConfig();
        }
    }
}
