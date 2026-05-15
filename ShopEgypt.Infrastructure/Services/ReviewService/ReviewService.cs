using Mapster;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Application.DTOs.Admin;
using ShopEgypt.Application.DTOs.ReviewDtos;
using ShopEgypt.Application.Interfaces.IReviewService;
using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.Services.ReviewService
{
    public class ReviewService: IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> GetByProductReviewsAsync(int productId, CancellationToken ct = default)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Product)        
                .Include(r => r.ApplicationUser)
                .ToListAsync(ct);

            return reviews.Adapt<IEnumerable<ReviewDto>>();
        }

        //public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, CancellationToken ct = default)
        //{
        //    var review = dto.Adapt<Review>();
        //    review.CreatedAt = DateTime.UtcNow;

        //    await _context.Reviews.AddAsync(review, ct);

        //    // reload with navigation properties for mapping
        //    var created = await _context.Reviews
        //        .Include(r => r.Product)
        //        .Include(r => r.ApplicationUser)
        //        .FirstAsync(r => r.Id == review.Id, ct);

        //    return created.Adapt<ReviewDto>();
        //}

        public async Task<bool> CreateReviewAsync(CreateReviewDto dto, CancellationToken ct = default)
        {
            var review = dto.Adapt<Review>();
            review.CreatedAt = DateTime.UtcNow;

            var entry = await _context.Reviews.AddAsync(review, ct);
            await _context.SaveChangesAsync(ct);

            // entry.Entity.Id is now populated after save
            var created = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.Id == entry.Entity.Id, ct);

            if (created is null)
                //throw new Exception($"Review with Id {entry.Entity.Id} not found after save.");
                return false;
 

            return true;
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id, CancellationToken ct = default)
        {
            var review = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.Id == id, ct);

            return review?.Adapt<ReviewDto>();
        }

        public async Task<bool> UpdateReviewAsync(UpdateReviewDto dto, CancellationToken ct = default)
        {
            var review = await _context.Reviews.FindAsync(new object[] { dto.Id }, ct);
            if (review == null || review.ApplicationUserId != dto.ApplicationUserId)
            {
                return false;
            }

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            await _context.SaveChangesAsync(ct);
            return true;
        }


        public async Task<bool> CheckExistingReviewAsync(int productId, string userId, CancellationToken ct = default)
        {
            return await _context.Reviews.AnyAsync(r => r.ProductId == productId && r.ApplicationUserId == userId, ct);
        }

        public async Task<AdminReviewsPageDto> GetAdminReviewsPageAsync(string? search = null, int? rating = null, CancellationToken ct = default)
        {
            var query = _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.ApplicationUser)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(r =>
                    r.Comment.Contains(search) ||
                    r.Product.Title.Contains(search) ||
                    r.ApplicationUser.Email.Contains(search) ||
                    r.ApplicationUser.UserName.Contains(search) ||
                    ((r.ApplicationUser.FirstName + " " + r.ApplicationUser.LastName).Contains(search)));
            }

            if (rating.HasValue && rating.Value >= 1 && rating.Value <= 5)
            {
                query = query.Where(r => r.Rating == rating.Value);
            }

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync(ct);

            var dto = new AdminReviewsPageDto
            {
                TotalReviews = reviews.Count,
                AverageRating = reviews.Count == 0 ? null : Math.Round(reviews.Average(r => r.Rating), 1),
                FiveStarReviews = reviews.Count(r => r.Rating == 5),
                Search = search,
                RatingFilter = rating,
                Reviews = reviews.Select(r =>
                {
                    var customerName = $"{r.ApplicationUser?.FirstName} {r.ApplicationUser?.LastName}".Trim();

                    if (string.IsNullOrWhiteSpace(customerName))
                        customerName = r.ApplicationUser?.UserName ?? r.ApplicationUser?.Email ?? "Unknown Customer";

                    return new AdminReviewListItemDto
                    {
                        Id = r.Id,
                        ProductId = r.ProductId,
                        ProductName = r.Product?.Title ?? $"Product #{r.ProductId}",
                        UserId = r.ApplicationUserId,
                        CustomerName = customerName,
                        CustomerEmail = r.ApplicationUser?.Email ?? string.Empty,
                        Rating = r.Rating,
                        Comment = r.Comment ?? string.Empty,
                        CreatedAt = r.CreatedAt
                    };
                }).ToList()
            };

            return dto;
        }

        public async Task<bool> DeleteReviewAsync(int id, CancellationToken ct = default)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id, ct);
            if (review == null)
                return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(ct);
            return true;
        }
    }

}

