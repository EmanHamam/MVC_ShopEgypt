using Microsoft.AspNetCore.Identity;
using ShopEgypt.Domain.Entities;

namespace ShopEgypt.Areas.Identity
{
    public static class ApplicationDbContextSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            //var adminEmail = "adminn@shopegypt.com";

            //var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            //if (existingAdmin == null)
            //{
            //    var admin = new ApplicationUser
            //    {
            //        UserName = adminEmail,
            //        Email = adminEmail,
            //        EmailConfirmed = true
            //    };

            //    var result = await userManager.CreateAsync(admin, "Admin@123");

            //    if (result.Succeeded)
            //    {
            //        await userManager.AddToRoleAsync(admin, "Admin");
            //    }
            //}
        }
    }
}
