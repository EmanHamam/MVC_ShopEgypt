using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Areas.Adminn.Models.Dashboard;
using ShopEgypt.Data.Context;

namespace ShopEgypt.Areas.Adminn.Controllers
{
    [Area("Adminn")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var topProducts = await _context.OrderItems
                .Include(oi => oi.Product)
                    .ThenInclude(p => p.ProductImages)
                .GroupBy(oi => new
                {
                    oi.ProductId,
                    oi.Product.Title,
                    oi.Product.Price,
                    oi.Product.DiscountPrice
                })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    Name = g.Key.Title,
                    SoldCount = g.Sum(x => x.Quantity),
                    UnitPrice = g.Key.DiscountPrice.HasValue && g.Key.DiscountPrice.Value > 0
                        ? g.Key.DiscountPrice.Value
                        : g.Key.Price,
                    ImageUrl = g.SelectMany(x => x.Product.ProductImages)
                                .Select(pi => pi.ImageUrl)
                                .FirstOrDefault()
                })
                .OrderByDescending(x => x.SoldCount)
                .Take(4)
                .ToListAsync();

            var model = new DashboardIndexViewModel
            {
                Stats = new List<DashboardStatViewModel>
                {
                    new() { Label = "Revenue", Value = "284,920 EGP", Delta = "+12.4%", IsUp = true, IconClass = "fa-solid fa-dollar-sign" },
                    new() { Label = "Orders", Value = "1,284", Delta = "+8.1%", IsUp = true, IconClass = "fa-solid fa-bag-shopping" },
                    new() { Label = "Customers", Value = "12,402", Delta = "+18.7%", IsUp = true, IconClass = "fa-solid fa-users" },
                    new() { Label = "Avg. Order", Value = "684 EGP", Delta = "-2.1%", IsUp = false, IconClass = "fa-solid fa-arrow-trend-up" }
                },

                TopProducts = topProducts
                    .Select((p, index) => new TopProductViewModel
                    {
                        Rank = index + 1,
                        ProductId = p.ProductId,
                        Name = p.Name,
                        ImageUrl = string.IsNullOrEmpty(p.ImageUrl) ? "/images/placeholder.png" : p.ImageUrl,
                        SoldCount = p.SoldCount,
                        TotalAmount = (p.UnitPrice * p.SoldCount).ToString("N0")
                    })
                    .ToList(),

                RecentOrders = new List<RecentOrderViewModel>
                {
                    new() { OrderCode = "SE-2104", CustomerName = "Mariam K.", Total = "3,450 EGP", Status = "Processing" },
                    new() { OrderCode = "SE-2103", CustomerName = "Ahmed F.", Total = "1,290 EGP", Status = "Shipped" },
                    new() { OrderCode = "SE-2102", CustomerName = "Yara N.", Total = "980 EGP", Status = "Delivered" }
                }
            };

            return View(model);
        }
    }
}