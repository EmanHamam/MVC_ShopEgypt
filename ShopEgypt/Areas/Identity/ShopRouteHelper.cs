namespace ShopEgypt.Areas.Identity
{
    /// <summary>
    /// Fixed storefront paths so link generation never picks the Adminn area route.
    /// </summary>
    internal static class ShopRouteHelper
    {
        public const string ShopPath = "/products";
        public const string CartPath = "/Cart";
        public const string WishlistPath = "/Wishlist";
        public const string CategoriesPath = "/Category";
        public const string MyOrdersPath = "/Orders/MyOrders";
        public const string AboutPath = "/about";
        public const string HomePath = "/";
        public const string AdminDashboardPath = "/Adminn/Dashboard/Index";

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
