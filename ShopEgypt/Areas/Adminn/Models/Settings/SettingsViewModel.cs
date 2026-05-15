namespace ShopEgypt.Areas.Adminn.Models.Settings
{
    public class SettingsViewModel
    {
        public string StoreName { get; set; } = string.Empty;
        public string SupportEmail { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public decimal FreeShippingThreshold { get; set; }
        public decimal StandardRate { get; set; }
    }
}
