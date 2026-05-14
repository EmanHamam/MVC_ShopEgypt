using ShopEgypt.Application.DTOs.Admin;

namespace ShopEgypt.Areas.Adminn.Models.Products
{
    public class AdminProductListPageViewModel
    {
        public List<AdminProductListItemDto> Products { get; set; } = new();
        public int TotalCount { get; set; }

        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public string? Status { get; set; }

        public List<AdminSelectItemViewModel> Categories { get; set; } = new();
    }
}
