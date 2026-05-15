namespace ShopEgypt.Areas.Adminn.Models.Dashboard
{
    public class TopProductViewModel
    {
        public int Rank { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int SoldCount { get; set; }
        public string TotalAmount { get; set; } = string.Empty;
    }
}   