using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShopEgypt.Application.DTOs.WishlistItemsDTOs
{
    public class AddWishlistItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
    }
}
