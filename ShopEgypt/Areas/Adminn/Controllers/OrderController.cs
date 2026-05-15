using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.Interfaces.IOrderService;

namespace ShopEgypt.Areas.Adminn.Controllers
{
    [Area("Adminn")]
    [Authorize(Roles = "Admin")]
    public class OrdersController(IOrderService orderService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var orders = await orderService.GetAllOrdersForAdminAsync();
            return View(orders);
        }
    }
}
