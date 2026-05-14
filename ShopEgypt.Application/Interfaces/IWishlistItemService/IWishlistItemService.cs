using ShopEgypt.Application.DTOs.WishlistDTOs;
using ShopEgypt.Application.DTOs.WishlistItemsDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.Interfaces.IWishlistItemService
{
    public interface IWishlistItemService
    {
        Task<IEnumerable<WishlistItemsDto>> GetWishlistItemsByUserId(string UserID, CancellationToken ct = default);

        Task<int> RemoveWishlistItem(int ProductId, int WishlistId, CancellationToken ct = default);

        Task<WishlistItemsDto> AddWishlistItem(AddWishlistItemDto addWishlistItemDto, CancellationToken ct = default);
    }
}
