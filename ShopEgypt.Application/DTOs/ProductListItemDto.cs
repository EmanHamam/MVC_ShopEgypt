namespace ShopEgypt.Application.DTOs
{
    public class ProductListItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public bool IsOutOfStock { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
