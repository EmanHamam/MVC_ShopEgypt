using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Areas.Identity;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.UnitOfWork;
using ShopEgypt.ViewModels.Header;
using System.Security.Claims;

namespace ShopEgypt.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _uow;

        public HeaderViewComponent(ICartService cartService, UserManager<ApplicationUser> userManager, IUnitOfWork uow)
        {
            _cartService = cartService;
            _userManager = userManager;
            _uow = uow;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? userGreetingName = null;
            int wishlistCount = 0;

            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var appUser = await _userManager.GetUserAsync(HttpContext.User);
                if (appUser != null)
                {
                    userGreetingName = !string.IsNullOrWhiteSpace(appUser.FirstName)
                        ? appUser.FirstName.Trim()
                        : (!string.IsNullOrWhiteSpace(appUser.Email) ? appUser.Email : appUser.UserName);

                    // Get wishlist count for the current user
                    var userId = UserClaimsPrincipal?.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var wishlistItems = await _uow.WishlistItemService.GetWishlistItemsByUserId(userId);
                        wishlistCount = wishlistItems?.Count() ?? 0;
                    }
                }
            }

            var model = new HeaderViewModel
            {
                CartCount = await _cartService.GetCartCountAsync(),
                WishlistCount = wishlistCount,
                UserGreetingName = userGreetingName,
                MyOrdersUrl = AuthRedirectHelper.MyOrdersPath
            };

            return View(model);
        }
    }
}
