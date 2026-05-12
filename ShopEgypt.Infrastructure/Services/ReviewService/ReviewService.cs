using Mapster;
using Microsoft.EntityFrameworkCore;
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
    internal class ReviewService: IReviewService
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

    }
}
