using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Data.Context;
using ShopEgypt.Application.Mappings;
using ShopEgypt.Infrastructure.ServiceRegistration;
using ShopEgypt.Domain.Entities;

namespace ShopEgypt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //Services Registration

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //       .AddEntityFrameworkStores<ApplicationDbContext>()
            //       .AddDefaultTokenProviders();

            builder
                .Services.AddDefaultIdentity<ApplicationUser>(options =>
                    options.SignIn.RequireConfirmedAccount = true
                )
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddControllersWithViews();

            MapsterConfig.RegisterMappings();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                )
                .WithStaticAssets();
            app.MapRazorPages().WithStaticAssets();

            app.Run();
        }
    }
}
