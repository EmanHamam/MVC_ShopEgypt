using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.ViewModels.Header;

namespace ShopEgypt.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
       // private readonly IWishlistService _wishlistService;

        public HeaderViewComponent(ICartService cartService)
        {
            _cartService = cartService;
           // _wishlistService = wishlistService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new HeaderViewModel
            {
                CartCount = await _cartService.GetCartCountAsync(),
                //WishlistCount = _wishlistService.GetWishlistCount()
            };

            return View(model);
        }
    }
}
