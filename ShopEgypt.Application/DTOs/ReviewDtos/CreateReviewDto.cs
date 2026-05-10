using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.ReviewDtos
{
    public class CreateReviewDto
    {
        public int ProductId { get; set; }

        public string ApplicationUserId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}
