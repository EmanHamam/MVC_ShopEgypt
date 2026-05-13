using Mapster;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Application.DTOs.WishlistDTOs;
using ShopEgypt.Application.DTOs.WishlistItemsDTOs;
using ShopEgypt.Application.Interfaces.IWishlistItemService;
using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.Services.WishlistItemService
{
    public class WishlistItemService : IWishlistItemService
    {
        private readonly ApplicationDbContext _context;

        public WishlistItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WishlistItemsDto>> GetWishlistItemsByUserId(string UserID, CancellationToken ct = default)
        {
            var wishItems =  await _context.WishlistItems
            .Include(w => w.Product)
                .ThenInclude(p => p.ProductImages)
            .Include(w => w.Wishlist)
            .Where(w => w.Wishlist.ApplicationUserId == UserID)
            .ToListAsync(ct);

            return wishItems.Adapt<IEnumerable<WishlistItemsDto>>();
        }

        public async Task<int> RemoveWishlistItem(int ProductId, int WishlistId, CancellationToken ct = default)
        {
            return await _context.WishlistItems
                .Where(w => w.ProductId == ProductId && w.WishlistID == WishlistId)
                .ExecuteDeleteAsync(ct);
        }

        public async Task<WishlistItemsDto> AddWishlistItem(AddWishlistItemDto addWishlistItemDto, CancellationToken ct = default)
        {
            // Get or create wishlist for the user
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.ApplicationUserId == addWishlistItemDto.ApplicationUserId);

            if (wishlist == null)
            {
                // Create a new wishlist for the user
                wishlist = new Wishlist
                {
                    ApplicationUserId = addWishlistItemDto.ApplicationUserId
                };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync(ct);
            }

            // Check if the item already exists in the wishlist
            var existingItem = await _context.WishlistItems
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(w => w.WishlistID == wishlist.ID && w.ProductId == addWishlistItemDto.ProductId, ct);

            if (existingItem != null)
            {
                // Item already exists, return it
                return existingItem.Adapt<WishlistItemsDto>();
            }

            // Create new wishlist item - only need ProductId and WishlistID
            var wishlistItem = new WishlistItem
            {
                WishlistID = wishlist.ID,
                ProductId = addWishlistItemDto.ProductId
            };

            // Set the shadow property ApplicationUserId using EF Core's Entry API
            _context.WishlistItems.Add(wishlistItem);
            
            await _context.SaveChangesAsync(ct);

            // Load the related data for the response
            var addedItem = await _context.WishlistItems
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(w => w.Id == wishlistItem.Id, ct);

            return addedItem.Adapt<WishlistItemsDto>();
        }
    }
}
