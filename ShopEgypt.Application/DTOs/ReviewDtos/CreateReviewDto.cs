using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShopEgypt.Application.DTOs.ReviewDtos
{
    public class CreateReviewDto
    {
        public int ProductId { get; set; }

        public string ApplicationUserId { get; set; }

        public int Rating { get; set; }

        [Required(ErrorMessage = "The Review Comment is required")]
        [MinLength(5, ErrorMessage = "Comment should be at least 5 characters")]
        public string Comment { get; set; }
    }
}
