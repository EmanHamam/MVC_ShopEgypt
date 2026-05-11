using ShopEgypt.Application.DTOs.ReviewDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.Interfaces.IReviewService
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetByProductReviewsAsync(int productId, CancellationToken ct = default);
        //Task<double> GetAverageRatingAsync(int productId, CancellationToken ct = default);

        Task<bool> CreateReviewAsync(CreateReviewDto dto, CancellationToken ct = default);
        Task<ReviewDto?> GetReviewByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateReviewAsync(UpdateReviewDto dto, CancellationToken ct = default);
        //Task DeleteAsync(int id, string requestingUserId, CancellationToken ct = default);

    }
}
