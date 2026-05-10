using ShopEgypt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.Interfaces.ICartService
{
    public interface ICartService
    {
        Task<List<CartItem>> GetCartAsync();
        Task AddToCartAsync(int productId, int qty = 1);
        Task RemoveFromCartAsync(int productId);
        Task UpdateQuantityAsync(int productId, int qty);
        Task<int> GetCartCountAsync();
        Task<decimal> GetSubtotalAsync();
        Task<decimal> GetShippingAsync();
        Task<decimal> GetTotalAsync();
        Task ClearCartAsync();
        Task MergeSessionCartToUserCartAsync();
    }
}
