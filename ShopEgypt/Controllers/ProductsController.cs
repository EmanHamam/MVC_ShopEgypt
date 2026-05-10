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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12, int? categoryId = null, ProductSortBy? sortBy = null, string? keyWord = null, CancellationToken cancellationToken = default)
        {
            var result = await _productService.GetAllProductsAsync(page, pageSize, categoryId, sortBy, keyWord, cancellationToken);
            var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SortBy = sortBy;
            ViewBag.KeyWord = keyWord;
            return View(result);
        }

        // GET: Products/Details/5
        [HttpGet]
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            return View(new Product());
        }


        // POST: Products/Create
        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product,List<IFormFile>? images,CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (images != null && images.Count > 0)
            {
                product.ProductImages ??= new List<ProductImage>();
                var order = 0;
                foreach (var image in images)
                {
                    if (image.Length == 0)
                    {
                        continue;
                    }

                    var upload = await _imageStorage.UploadAsync(
                        image.OpenReadStream(),
                        image.FileName,
                        image.ContentType,
                        "products",
                        cancellationToken
                    );

                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = upload.Url,
                        IsThumbnail = order == 0,
                        DisplayOrder = order
                    });

                    order++;
                }
            }

            var created = await _productService.CreateProductAsync(product, cancellationToken);
            return RedirectToAction(nameof(Details), new { id=created.Id });
        }

        // GET: Products/Edit/5
        [HttpGet]
        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetProductEntityByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [Route("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product,List<IFormFile>? images,CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            if (images != null && images.Count > 0)
            {
                product.ProductImages ??= new List<ProductImage>();
                var order = await _productService.GetNextImageOrderAsync(product.Id, cancellationToken);

                foreach (var image in images)
                {
                    if (image.Length == 0)
                    {
                        continue;
                    }

                    var upload = await _imageStorage.UploadAsync(
                        image.OpenReadStream(),
                        image.FileName,
                        image.ContentType,
                        "products",
                        cancellationToken
                    );

                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = upload.Url,
                        IsThumbnail = order == 0,
                        DisplayOrder = order
                    });

                    order++;
                }
            }

            await _productService.UpdateProductAsync(product, cancellationToken);
            return RedirectToAction(nameof(Details), new { id=product.Id });
        }

        // GET: Products/Delete/5
        [HttpGet]
        [Route("delete/{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetProductByIdAsync(id, cancellationToken);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [Route("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Product product, CancellationToken cancellationToken = default)
        {
            await _productService.DeleteProductAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
    }
}
