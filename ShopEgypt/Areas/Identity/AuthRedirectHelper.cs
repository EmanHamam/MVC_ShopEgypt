namespace ShopEgypt.Areas.Identity
{
    internal static class AuthRedirectHelper
    {
        public const string ShopPath = "/products";
        public const string AdminDashboardPath = "/Adminn/Dashboard/Index";
        public const string MyOrdersPath = "/Orders/MyOrders";

        public static bool IsAdminAreaUrl(string? url)
        {
            return !string.IsNullOrWhiteSpace(url)
                && url.StartsWith("/Adminn", StringComparison.OrdinalIgnoreCase);
        }

        public static string SanitizeReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || IsAdminAreaUrl(returnUrl))
            {
                return ShopPath;
            }

            return returnUrl;
        }
    }
}
