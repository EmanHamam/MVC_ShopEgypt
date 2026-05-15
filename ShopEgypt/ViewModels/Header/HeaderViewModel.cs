namespace ShopEgypt.ViewModels.Header
{
    public class HeaderViewModel
    {
        public int CartCount { get; set; }
        public int WishlistCount { get; set; }

        public string? UserGreetingName { get; set; }

        public string ShopUrl { get; set; } = "/products";
        public string CategoriesUrl { get; set; } = "/Category";
        public string MyOrdersUrl { get; set; } = "/Orders/MyOrders";
        public string AboutUrl { get; set; } = "/Home/About";
        public string HomeUrl { get; set; } = "/";
        public string CartUrl { get; set; } = "/Cart";
        public string WishlistUrl { get; set; } = "/Wishlist";
    }
}
