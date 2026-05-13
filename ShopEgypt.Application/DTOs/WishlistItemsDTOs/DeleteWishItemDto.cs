using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.WishlistItemsDTOs
{
    public class DeleteWishItemDto
    {
        public int ProductId { get; set; }

        public int WishlistId { get; set; }
    }
}
