using ShopEgypt.Application.Interfaces.IProductService;
using ShopEgypt.Application.Interfaces.IReviewService;
﻿using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IReviewService ReviewService { get; }
        IProductService ProductService { get; }

        IGenericRepository<Address> Addresses { get; }
        IGenericRepository<ApplicationUser> ApplicationUsers { get; }
        IGenericRepository<Brand> Brands { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Cart> Carts { get; }
        IGenericRepository<CartItem> CartItems { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Payment> Payments { get; }
        IGenericRepository<ProductImage> ProductImages { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<Wishlist> WishLists { get; }
        IGenericRepository<WishlistItem> WishlistItems { get; }

        Task<int> SaveAsync();
    }
}
