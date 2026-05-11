using ShopEgypt.Application.Interfaces.IProductService;
using ShopEgypt.Application.Interfaces.IReviewService;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        IReviewService ReviewService { get; }
        IProductService ProductService { get; }
        Task<int> SaveAllAsync(CancellationToken cancellationToken = default);
    }
}
