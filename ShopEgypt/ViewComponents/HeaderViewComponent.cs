using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Domain.Entities;
using ShopEgypt.ViewModels.Header;

namespace ShopEgypt.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HeaderViewComponent(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string? userGreetingName = null;
            if (HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var appUser = await _userManager.GetUserAsync(HttpContext.User);
                if (appUser != null)
                {
                    userGreetingName = !string.IsNullOrWhiteSpace(appUser.FirstName)
                        ? appUser.FirstName.Trim()
                        : (!string.IsNullOrWhiteSpace(appUser.Email) ? appUser.Email : appUser.UserName);
                }
            }

            var model = new HeaderViewModel
            {
                CartCount = await _cartService.GetCartCountAsync(),
                UserGreetingName = userGreetingName
            };

            return View(model);
        }
    }
}
