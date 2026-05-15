using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopEgypt.Application.Interfaces.IAddressService;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Application.Interfaces.ICategoryService;
using ShopEgypt.Application.Interfaces.ICustomerService;
using ShopEgypt.Application.Interfaces.IImageStorageService;
using ShopEgypt.Application.Interfaces.IOrderService;
using ShopEgypt.Application.Interfaces.IProductService;
using ShopEgypt.Application.Interfaces.IReviewService;
using ShopEgypt.Application.Interfaces.IStripeService;
using ShopEgypt.Application.Interfaces.IWishlistItemService;
using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.ExternalServices.SendGridEmailSender;
using ShopEgypt.Infrastructure.ExternalServices.StripeService;
using ShopEgypt.Infrastructure.Services.AddressService;
using ShopEgypt.Infrastructure.Services.CartService;
using ShopEgypt.Infrastructure.Services.CategoryService;
using ShopEgypt.Infrastructure.Services.CloudinaryService;
using ShopEgypt.Infrastructure.Services.CustomerService;
using ShopEgypt.Infrastructure.Services.OrderService;
using ShopEgypt.Infrastructure.Services.ProductService;
using ShopEgypt.Infrastructure.Services.ReviewService;
using ShopEgypt.Infrastructure.Services.WishlistItemService;
using ShopEgypt.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using StripeConfig = Stripe.StripeConfiguration;

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
            services.AddTransient<IEmailSender, SendGridEmailSender>();

            
            services.AddAuthentication().
        AddGoogle(options =>
        {
            options.ClientId = configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId not found in configuration.");
            options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not found in configuration.");
            options.CallbackPath = "/signin-google";
        }).AddFacebook(options => {
            options.AppId = configuration["Authentication:Facebook:AppId"] ?? throw new InvalidOperationException("Facebook AppId not found in configuration.");
            options.AppSecret = configuration["Authentication:Facebook:AppSecret"] ?? throw new InvalidOperationException("Facebook AppSecret not found in configuration.");
        });

            // Configure Stripe Dev — key is read once at startup from user secrets / env vars
            //StripeConfig.ApiKey = configuration.GetSection("Stripe")["SecretKey"] 
            //    ?? throw new InvalidOperationException("Stripe SecretKey not found in configuration.");


            //Application Services Registration
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStripeService, StripeService>();
            services.AddScoped<IWishlistItemService, WishlistItemService>();
            services.AddScoped<ICustomerService, CustomerService>();

            // Auto Mapper
            //services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            });
            return services;
        }
    }
}
