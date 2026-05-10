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

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, CancellationToken ct = default)
        {
            var review = dto.Adapt<Review>();
            review.ApplicationUserId = "267824f8-683c-4812-afc8-1e1dbdafe519";
            review.CreatedAt = DateTime.UtcNow;

            var entry = await _context.Reviews.AddAsync(review, ct);
            await _context.SaveChangesAsync(ct);

            // entry.Entity.Id is now populated after save
            var created = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.Id == entry.Entity.Id, ct);

            if (created is null)
                throw new Exception($"Review with Id {entry.Entity.Id} not found after save.");

            return created.Adapt<ReviewDto>();
        }

    }
}
