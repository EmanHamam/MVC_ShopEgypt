using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.DTOs.WishlistItemsDTOs;
using ShopEgypt.Application.Interfaces.IWishlistItemService;
using ShopEgypt.Infrastructure.UnitOfWork;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ShopEgypt.Controllers
{
    [Authorize]
    [Route("Wishlist")]
    public class WishListItemsController : Controller
    {
        private readonly IUnitOfWork _uow;

        public WishListItemsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var wishlistItems = await _uow.WishlistItemService.GetWishlistItemsByUserId(userId);
            return View(wishlistItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Add")]
        public async Task<IActionResult> Add(int productId, string returnUrl = null, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var addWishlistItemDto = new AddWishlistItemDto
            {
                ProductId = productId,
                ApplicationUserId = userId
            };

            var result = await _uow.WishlistItemService.AddWishlistItem(addWishlistItemDto, cancellationToken);
            
            TempData["SuccessMessage"] = "Item added to wishlist successfully.";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Remove")]
        public async Task<IActionResult> Remove(int productId, int wishlistId, CancellationToken cancellationToken = default)
        {
            await _uow.WishlistItemService.RemoveWishlistItem(productId, wishlistId);
            TempData["SuccessMessage"] = "Item removed from wishlist successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
