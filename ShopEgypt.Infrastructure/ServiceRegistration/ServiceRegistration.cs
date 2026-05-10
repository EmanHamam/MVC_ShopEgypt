using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Application.Interfaces.IImageStorageService;
using ShopEgypt.Application.Interfaces.IProductService;
using ShopEgypt.Application.Interfaces.IReviewService;
using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.Services.CartService;
using ShopEgypt.Infrastructure.Services.CloudinaryService;
using ShopEgypt.Infrastructure.Services.ProductService;
using ShopEgypt.Infrastructure.Services.ReviewService;
using ShopEgypt.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found."
                );

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //       .AddEntityFrameworkStores<ApplicationDbContext>()
            //       .AddDefaultTokenProviders();

            //External Services Registration
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();

            //Application Services Registration
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IReviewService, ReviewService>();


            // Auto Mapper
            //services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            return services;
        }
    }
}
