using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.CartDTO
{
    public class CartDisplayItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Qty { get; set; }
    }
}
