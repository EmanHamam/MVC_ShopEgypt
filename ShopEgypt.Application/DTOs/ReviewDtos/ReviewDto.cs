using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.ReviewDtos
{
    public class ReviewDto
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ApplicationUserId { get; set; }

        public string UserName { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
