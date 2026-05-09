using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShopEgypt.Application.Interfaces.ICartService;
using ShopEgypt.Data.Context;
using System.Threading.Tasks;
using System.Linq;

namespace ShopEgypt.Infrastructure.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public CartService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public decimal CalculateShipping(string governorate)
        {
            if (string.IsNullOrEmpty(governorate))
            {
                return _configuration.GetValue<decimal>("ShippingFees:Default", 100m);
            }

            // Since governorates from map often come back as "Cairo Governorate", we just match partially or use the exact key if configured.
            var feeStr = _configuration.GetSection("ShippingFees")[governorate];
            if (decimal.TryParse(feeStr, out decimal fee))
            {
                return fee;
            }

            return _configuration.GetValue<decimal>("ShippingFees:Default", 100m);
        }

        public async Task<bool> IsCartEmptyAsync(string userId)
        {
            // Because we are currently mocking cart items later in the checkout, 
            // for the sake of letting you test the flow, we will return false here.
            // When you are ready to use the real database, uncomment this logic:
            
            /*
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return true; 
            }
            */

            return false; 
        }
    }
}
