using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Domain.Entities;
using ShopEgypt.ViewModels.Cart;

namespace ShopEgypt.Controllers.Cart
{
    [Route("Cart")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;

        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var cartItems = await _cartService.GetCartAsync();
            var subtotal = await _cartService.GetSubtotalAsync();
            var shipping = await _cartService.GetShippingAsync();

            var model = new CartIndexViewModel
            {
                Items = cartItems.Select(x => new CartItemVm
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product?.Title ?? "Product",
                    ImageUrl = x.Product?.ProductImages?.FirstOrDefault()?.ImageUrl ?? "/images/placeholder.png",
                    UnitPrice = x.Product != null
                        ? (x.Product.DiscountPrice.HasValue && x.Product.DiscountPrice.Value > 0
                            ? x.Product.DiscountPrice.Value
                            : x.Product.Price)
                        : 0,
                    Quantity = x.Quantity
                }).ToList(),
                Subtotal = subtotal,
                Shipping = shipping
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Add")]
        public async Task<IActionResult> Add(int productId, int qty = 1, string? returnUrl = null)
        {
            await _cartService.AddToCartAsync(productId, qty);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return LocalRedirect("/products");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Increase")]
        public async Task<IActionResult> Increase(int productId)
        {
            var cartItems = await _cartService.GetCartAsync();
            var item = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                await _cartService.UpdateQuantityAsync(productId, item.Quantity + 1);
            }

            return LocalRedirect("/Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Decrease")]
        public async Task<IActionResult> Decrease(int productId)
        {
            var cartItems = await _cartService.GetCartAsync();
            var item = cartItems.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                var newQty = item.Quantity - 1;
                await _cartService.UpdateQuantityAsync(productId, newQty);
            }

            return LocalRedirect("/Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Remove")]
        public async Task<IActionResult> Remove(int productId)
        {
            await _cartService.RemoveFromCartAsync(productId);
            return LocalRedirect("/Cart");
        }
    }
}
