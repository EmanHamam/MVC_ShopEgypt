using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.WishlistDTOs
{
    public class WishlistItemsDto
    {
        public int Id { get; set; }

        public int WishlistId { get; set; }

        public int ProductId  { get; set; }

        public string ProductName { get; set; }

        public string? ProductImage { get; set; }
    }
}
