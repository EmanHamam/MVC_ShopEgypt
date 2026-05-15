namespace ShopEgypt.Areas.Adminn.Models.Dashboard
{
    public class DashboardIndexViewModel
    {
        public List<DashboardStatViewModel> Stats { get; set; } = new();
        public List<TopProductViewModel> TopProducts { get; set; } = new();
        public List<RecentOrderViewModel> RecentOrders { get; set; } = new();
    }
}