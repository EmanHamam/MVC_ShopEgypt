using ShopEgypt.Application.Interfaces.IReviewService;
using ShopEgypt.Data.Context;
using ShopEgypt.Infrastructure.Services.ReviewService;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.UnitOfWork
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // Add your services and 
        public IReviewService ReviewService { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ReviewService = new ReviewService(_context);
        }

        public async Task<int> SaveAllAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }
    }
}
