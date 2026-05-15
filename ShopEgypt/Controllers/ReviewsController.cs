using Mapster;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.DTOs.ReviewDtos;
using ShopEgypt.Infrastructure.UnitOfWork;
using System.Text.Json;

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
        //public IActionResult Create(int productId)
        //{
        //    var dto = new CreateReviewDto { ProductId = productId };
        //    return View(dto);
        //}

        // POST /Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateReviewDto dto, CancellationToken ct)
        {
            
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine(currentUserId);
            if (string.IsNullOrEmpty(currentUserId))
            {
                
                return Challenge();
            }

            dto.ApplicationUserId = currentUserId;

            if (!ModelState.IsValid)
            {
                foreach (var key in Request.Form.Keys)
                {
                    Console.WriteLine($"{key} = {Request.Form[key]}");
                }
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key}");
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }

                var product = await _uow.ProductService.GetProductByIdAsync(dto.ProductId, ct);

                product.CreateReview = dto;

                return View("~/Views/Products/Details.cshtml", product);
            }

            // if user alredy has review for this product, redirect to details page with error message
            var existinReview = await _uow.ReviewService.CheckExistingReviewAsync(dto.ProductId, currentUserId, ct);
            if (existinReview)
            {
                // Add error message to ModelState
                ModelState.AddModelError("", "You have already reviewed this product.");
                TempData["Error"] = "You Already submitted a review for this product.";
                var product = await _uow.ProductService.GetProductByIdAsync(dto.ProductId, ct);
                product.CreateReview = dto;
                return View("~/Views/Products/Details.cshtml", product);
            }

            var created = await _uow.ReviewService.CreateReviewAsync(dto, ct);
            if (!created)
            {
                throw new Exception($"Review not found after save.");
            }

            return RedirectToAction("Details", "Products", new { area = "", Id = dto.ProductId });
        }

        // GET /Reviews/Edit/5
        [HttpGet]
        [Route("reviews/edit/{id}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var review = await _uow.ReviewService.GetReviewByIdAsync(id, ct);
            if (review == null)
            {
                return NotFound();
            }

            // Check if current user owns the review
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (review.ApplicationUserId != currentUserId)
            {
                return Forbid();
            }

            var dto = review.Adapt<UpdateReviewDto>();

            return View(dto);
        }

        // POST /Reviews/Edit/5
        [HttpPost]
        [Route("reviews/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateReviewDto dto, CancellationToken ct)
        {
            dto.Id = id; // Set Id from route

            if (!ModelState.IsValid)
            {
                
                return View(dto);
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            dto.ApplicationUserId = currentUserId;

            var updated = await _uow.ReviewService.UpdateReviewAsync(dto, ct);
            if (!updated)
            {
                ModelState.AddModelError("", "Failed to update review.");
                return View(dto);
            }

            return RedirectToAction("Details", "Products", new { area = "", Id = dto.ProductId });
        }
    }
}
