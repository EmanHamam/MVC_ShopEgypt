using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.DTOs.Admin
{
    public class AdminProductFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public string? Search { get; set; }
    }
}
