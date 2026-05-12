using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopEgypt.Application.Interfaces.ICategoryService;

namespace ShopEgypt.Controllers
{
    [Route("Category")]
    public class CategoryController : Controller
    {
        public ICategoryService CategoryService { get; set; }
        public CategoryController(ICategoryService categoryService)
        {
            CategoryService = categoryService;
        }
        // GET: CategoryController
        [HttpGet]
        [Route("")]
        public async Task<ActionResult> Index(CancellationToken cancellationToken)
        {
            var categories = await CategoryService.GetAllCategoriesMainAsync(cancellationToken);
            return View(categories);
        }
    }
}
