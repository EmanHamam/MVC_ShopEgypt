using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.Admin
{
    public class AdminProductListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? BrandName { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string? MainImageUrl { get; set; }
    }
}
