using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.Interfaces.ICustomerService;

namespace ShopEgypt.Areas.Adminn.Controllers
{
    [Area("Adminn")]
    [Authorize(Roles = "Admin")]
    public class CustomersController(ICustomerService customerService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var customers = await customerService.GetAllCustomersForAdminAsync();
            return View(customers);
        }
    }
}
