using ShopEgypt.Application.DTOs.ReviewDtos;
using System.Collections.Generic;

namespace ShopEgypt.Application.DTOs
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public IReadOnlyList<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();

        public List<ReviewDto> ProductReviews { get; set; } = new List<ReviewDto>();
        public CreateReviewDto CreateReview { get; set; } = new();
    }
}
