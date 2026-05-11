using System;
using System.Data;
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
                    .ToList())
                .Map(dest => dest.ProductReviews, src => src.Reviews);


            TypeAdapterConfig<Review, ReviewDto>
                .NewConfig()
                .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Title : null) // null-safe
                .Ignore(dest => dest.ApplicationUser)
                .Map(dest => dest.ApplicationUserId, src => src.ApplicationUserId)
                .Map(dest => dest.UserName, src => src.ApplicationUser.UserName)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt);


            // Create ReviewDto Mapping, no need to mapping as share the same properties
            TypeAdapterConfig<Review, CreateReviewDto>
                .NewConfig()
                .Map(dest => dest.ApplicationUserId, src => src.ApplicationUserId)
                .Map(dest => dest.ProductId, src => src.ProductId);

            TypeAdapterConfig<ReviewDto, UpdateReviewDto>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.ProductId, src => src.ProductId)
                .Map(dest => dest.ApplicationUserId, src => src.ApplicationUserId)
                .Map(dest => dest.Rating, src => src.Rating)
                .Map(dest => dest.Comment, src => src.Comment);
            // TypeAdapterConfig<Review, UpdateReviewDto>
            //     .NewConfig()
            //     .Map(dest => dest.Id, src => src.Id)
            //     .Map(dest => dest.ProductId, src => src.ProductId)
            //     .Map(dest => dest.ApplicationUserId, src => src.ApplicationUserId)
            //     .Map(dest => dest.Rating, src => src.Rating)
            //     .Map(dest => dest.Comment, src => src.Comment);

            TypeAdapterConfig<Category, CategoryDto>.NewConfig();
        }
    }
}
