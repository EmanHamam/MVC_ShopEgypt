using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.Interfaces.IProductService;
using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums.ProductEnums;

namespace ShopEgypt.Infrastructure.Services.ProductService
{
    public class ProductService : IProductService
    {
        public ApplicationDbContext Context { get; set; }
        public ProductService(ApplicationDbContext context) 
        {
            Context = context;
        }
        public async Task<PagedResultDto<ProductListItemDto>> GetAllProductsAsync(int pageNumber,int pageSize,int? categoryId, 
            ProductSortBy? sortBy,string? keyWord, decimal? minPrice, decimal? maxPrice, CancellationToken cancellationToken)
        {
            IQueryable<Product> query = Context.Products
                .AsNoTracking()
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.ApplicationUser);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            if (sortBy.HasValue)
            {
                switch (sortBy.Value)
                {
                    case ProductSortBy.Newest:
                        query = query.OrderBy(p => p.CreatedAt);
                        break;
                    case ProductSortBy.PriceLowToHigh:
                        query = query.OrderBy(p => p.Price);
                        break;
                    case ProductSortBy.PriceHighToLow:
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case ProductSortBy.Rated:
                        query = query.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(p => p.Rating) : 0);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(keyWord))
            {
                keyWord = keyWord.Trim().ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(keyWord) || p.Description.ToLower().Contains(keyWord));
            }
            if (minPrice != null && minPrice >= 0)
            {
                query = query.Where(p => p.Price >= minPrice);
            }
            if (maxPrice != null && maxPrice > 0)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResultDto<ProductListItemDto>
            {
                Items = products.Adapt<List<ProductListItemDto>>(),
                TotalCount = totalCount
            };
        }

        public async Task<ProductDetailDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
        {
            var product = await Context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.ApplicationUser)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            return product?.Adapt<ProductDetailDto>();
        }

        public async Task<Product?> GetProductEntityByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken)
        {
            product.CreatedAt = DateTime.UtcNow;
            Context.Products.Add(product);
            await Context.SaveChangesAsync(cancellationToken);
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product, CancellationToken cancellationToken)
        {
            int id = product.Id;
            var updatedproduct = await Context.Products.FindAsync(id, cancellationToken);
            if (updatedproduct == null)
            {
                throw new InvalidOperationException("Product not found");
            }
            product.Adapt(updatedproduct);
            await Context.SaveChangesAsync(cancellationToken);
            return updatedproduct;
        }

        public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await Context.Products.FindAsync(id, cancellationToken);
            if (product == null)
            { 
                return false;
            }
            Context.Products.Remove(product);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<int> GetNextImageOrderAsync(int productId, CancellationToken cancellationToken)
        {
            var maxOrder = await Context.ProductImages
                .AsNoTracking()
                .Where(pi => pi.ProductId == productId)
                .MaxAsync(pi => (int?)pi.DisplayOrder, cancellationToken);

            return (maxOrder ?? -1) + 1;
        }

    }
}
