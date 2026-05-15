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

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public List<AdminSelectItemViewModel> Categories { get; set; } = new();
    }
}
