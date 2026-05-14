using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.Admin
{
    public class AdminProductDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string SellerId { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
    }
}
