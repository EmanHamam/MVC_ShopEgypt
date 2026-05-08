namespace ShopEgypt.Application.DTOs
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsThumbnail { get; set; }
        public int DisplayOrder { get; set; }
    }
}
