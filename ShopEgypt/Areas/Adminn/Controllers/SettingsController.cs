using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Areas.Adminn.Models.Settings;

namespace ShopEgypt.Areas.Adminn.Controllers
{
    [Area("Adminn")]
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var model = new SettingsViewModel
            {
                StoreName = "ShopEgypt",
                SupportEmail = "hello@shopegypt.co",
                Currency = "EGP",
                FreeShippingThreshold = 1500,
                StandardRate = 80
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(SettingsViewModel model)
        {
            // Static only for now
            // No database save requested

            TempData["SuccessMessage"] = "Settings saved successfully.";
            return View(model);
        }
    }
}
