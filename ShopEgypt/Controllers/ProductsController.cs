using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShopEgypt.Application.Interfaces.ICategoryService;
using ShopEgypt.Application.Interfaces.IImageStorageService;
using ShopEgypt.Application.Interfaces.IProductService;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums.ProductEnums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShopEgypt.Controllers
{
    [Route("products")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IImageStorageService _imageStorage;
        private readonly ICategoryService _categoryService;

        public ProductsController(
            IProductService productService,
            IImageStorageService imageStorage,
            ICategoryService categoryService
        )
        {
            _productService = productService;
            _imageStorage = imageStorage;
            _categoryService = categoryService;
        }

        // GET: Products
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, int? categoryId = null, ProductSortBy? sortBy = null, string? keyWord = null, decimal? minPrice = null, decimal? maxPrice = null, CancellationToken cancellationToken = default)
        {
            var result = await _productService.GetAllProductsAsync(page, pageSize, categoryId, sortBy, keyWord, minPrice, maxPrice, cancellationToken);
            var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SortBy = sortBy;
            ViewBag.KeyWord = keyWord;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(result);
        }

        // GET: Products/Details/5
        [HttpGet]
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id, string? reviewSort = null, CancellationToken cancellationToken = default)
        {
            
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound();
            }

            // Sort reviews based on reviewSort parameter
            if (product.ProductReviews != null && product.ProductReviews.Any())
            {
                product.ProductReviews = reviewSort switch
                {
                    "highest" => product.ProductReviews.OrderByDescending(r => r.Rating).ToList(),
                    "lowest" => product.ProductReviews.OrderBy(r => r.Rating).ToList(),
                    "newest" => product.ProductReviews.OrderByDescending(r => r.CreatedAt).ToList(),
                    _ => product.ProductReviews.OrderByDescending(r => r.CreatedAt).ToList() // default to newest
                };
            }

            ViewBag.ReviewSort = reviewSort ?? "newest";
            return View(product);
        }

        

    }
}
