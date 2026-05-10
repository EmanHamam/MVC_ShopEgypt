namespace ShopEgypt.ViewModels.Cart
{
    public class CartItemVm
    {
        public int ProductId { get; set; }
        public int CartItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
