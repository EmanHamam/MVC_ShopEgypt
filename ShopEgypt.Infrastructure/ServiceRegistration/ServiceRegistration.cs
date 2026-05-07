using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Application.Mappings;
using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Infrastructure.Services.CartService;
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

            //Application Services Registration
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ICartService, CartService>();

            // Auto Mapper
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

            return services;
        }
    }
}
