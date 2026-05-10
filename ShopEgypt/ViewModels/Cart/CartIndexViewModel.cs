using ShopEgypt.Domain.Entities;

namespace ShopEgypt.ViewModels.Cart
{
    public class CartIndexViewModel
    {
        public List<CartItem> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total => Subtotal + Shipping;
        public int ItemCount => Items.Sum(x => x.Quantity);
    }
}
