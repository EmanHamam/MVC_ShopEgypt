using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShopEgypt.Application.DTOs.Admin
{
    public class AdminUpdateProductDto
    {
        [Required]
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public bool IsActive { get; set; }

        public string? Color { get; set; }
        public string? Size { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }
    }
}
