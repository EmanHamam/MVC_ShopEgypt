namespace ShopEgypt.Areas.Adminn.Models.Dashboard
{
    public class RecentOrderViewModel
    {
        public string OrderCode { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}