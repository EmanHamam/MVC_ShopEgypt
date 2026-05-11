using ShopEgypt.Application.Interfaces.IProductService;
using ShopEgypt.Application.Interfaces.IReviewService;
using ShopEgypt.Data.Context;
using ShopEgypt.Infrastructure.Services.ProductService;
using ShopEgypt.Infrastructure.Services.ReviewService;
﻿using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.Repositories;
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
        public IProductService ProductService { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ProductService = new ProductService(_context);
            ReviewService = new ReviewService(_context);
        }

        public async Task<int> SaveAllAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public IGenericRepository<Address> Addresses { get; private set; }
        public IGenericRepository<ApplicationUser> ApplicationUsers { get; private set; }
        public IGenericRepository<Brand> Brands { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Cart> Carts { get; private set; }
        public IGenericRepository<CartItem> CartItems { get; private set; }
        public IGenericRepository<Order> Orders { get; private set; }
        public IGenericRepository<OrderItem> OrderItems { get; private set; }
        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Payment> Payments { get; private set; }
        public IGenericRepository<ProductImage> ProductImages { get; private set; }
        public IGenericRepository<Review> Reviews { get; private set; }
        public IGenericRepository<Wishlist> WishLists { get; private set; }
        public IGenericRepository<WishlistItem> WishlistItems { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Addresses = new GenericRepository<Address>(context);
            ApplicationUsers = new GenericRepository<ApplicationUser>(context);
            Brands = new GenericRepository<Brand>(_context);
            Categories = new GenericRepository<Category>(_context);
            Carts = new GenericRepository<Cart>(_context);
            CartItems = new GenericRepository<CartItem>(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
            Products = new GenericRepository<Product>(_context);
            Payments = new GenericRepository<Payment>(_context);
            ProductImages = new GenericRepository<ProductImage>(_context);
            Reviews = new GenericRepository<Review>(_context);
            WishLists = new GenericRepository<Wishlist>(_context);
            WishlistItems = new GenericRepository<WishlistItem>(_context);
        }
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
