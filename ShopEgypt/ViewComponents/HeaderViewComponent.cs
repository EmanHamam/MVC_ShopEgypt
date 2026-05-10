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

        public IViewComponentResult Invoke()
        {
            var model = new HeaderViewModel
            {
                //CartCount = _cartService.GetCartCount(),
                //WishlistCount = _wishlistService.GetWishlistCount()
            };

            return View(model);
        }
    }
}
