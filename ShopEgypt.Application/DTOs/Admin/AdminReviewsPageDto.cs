using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.Admin
{
    public class AdminReviewsPageDto
    {
        public int TotalReviews { get; set; }
        public double? AverageRating { get; set; }
        public int FiveStarReviews { get; set; }

        public string? Search { get; set; }
        public int? RatingFilter { get; set; }

        public List<AdminReviewListItemDto> Reviews { get; set; } = new();
    }
}
