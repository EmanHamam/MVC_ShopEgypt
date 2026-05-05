using AutoMapper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Application.Mappings;
using ShopEgypt.Data.Context;
using ShopEgypt.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Infrastructure.ServiceRegistration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // Add connection string Setting
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));




            //services.AddIdentity<ApplicationUser, IdentityRole>()
            //       .AddEntityFrameworkStores<ApplicationDbContext>()
            //       .AddDefaultTokenProviders();




            //External Services Registration

            //Application Services Registration
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ICartService, ICartService>();


            // Auto Mapper
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());


            return services;
        }
        
    

}
}
