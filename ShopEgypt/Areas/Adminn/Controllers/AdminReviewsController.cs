using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.Interfaces.IReviewService;

namespace ShopEgypt.Areas.Adminn.Controllers
{
    [Area("Adminn")]
    [Authorize(Roles = "Admin")]
    public class AdminReviewsController(IReviewService reviewService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? rating, CancellationToken ct)
        {
            var model = await reviewService.GetAdminReviewsPageAsync(search, rating, ct);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await reviewService.DeleteReviewAsync(id, ct);
            return RedirectToAction("Index", "AdminReviews", new { area = "Adminn" });
        }
    }
}
