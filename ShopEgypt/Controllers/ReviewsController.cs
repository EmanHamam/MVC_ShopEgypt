using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.DTOs.ReviewDtos;
using ShopEgypt.Infrastructure.UnitOfWork;

namespace ShopEgypt.Controllers
{
    public class ReviewsController: Controller
    {

        private readonly IUnitOfWork _uow;

        public ReviewsController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // GET /Reviews/Create
        public IActionResult Create(int productId)
        {
            var dto = new CreateReviewDto { ProductId = productId };
            return View(dto);
        }

        // POST /Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewDto dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _uow.ReviewService.CreateReviewAsync(dto, ct);
            return RedirectToAction("Product", new { productId = dto.ProductId });
        }
    }
}
