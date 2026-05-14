namespace ShopEgypt.Areas.Adminn.Models.Dashboard
{
    public class DashboardStatViewModel
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Delta { get; set; } = string.Empty;
        public bool IsUp { get; set; }
        public string IconClass { get; set; } = string.Empty;
    }
}