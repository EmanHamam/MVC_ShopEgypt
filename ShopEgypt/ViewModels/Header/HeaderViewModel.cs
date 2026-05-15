namespace ShopEgypt.ViewModels.Header
{
    public class HeaderViewModel
    {
        public int CartCount { get; set; }
        public int WishlistCount { get; set; }

        /// <summary>First name (or email) for signed-in greeting.</summary>
        public string? UserGreetingName { get; set; }

        public string MyOrdersUrl { get; set; } = "/Orders/MyOrders";
    }
}
