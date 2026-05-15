using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Application.DTOs.Admin;
using ShopEgypt.Application.Interfaces.ICategoryService;
using ShopEgypt.Application.Interfaces.IProductService;
using ShopEgypt.Areas.Adminn.Models.Products;
using ShopEgypt.Data.Context;

namespace ShopEgypt.Areas.Adminn.Controllers
{
    [Area("Adminn")]
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ApplicationDbContext _context;

        public AdminProductsController(IProductService productService,ICategoryService categoryService,ApplicationDbContext context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search,int? categoryId,string? status,int pageNumber = 1,int pageSize = 10,CancellationToken cancellationToken = default)
        {
            bool? isActive = status?.ToLower() switch
            {
                "active" => true,
                "inactive" => false,
                _ => null
            };

            var result = await _productService.GetAdminProductsAsync(
                new AdminProductFilterDto
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    CategoryId = categoryId,
                    IsActive = isActive,
                    Search = search
                },
                cancellationToken);

            var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);

            var model = new AdminProductListPageViewModel
            {
                Products = result.Items,
                TotalCount = result.TotalCount,
                Search = search,
                CategoryId = categoryId,
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Categories = categories.Select(c => new AdminSelectItemViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return View(model);
        }
        
        
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var vm = new AdminProductFormViewModel
            {
                IsEdit = false,
                CreateDto = new AdminCreateProductDto()
            };

            await LoadLookupDataAsync(vm, cancellationToken);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminProductFormViewModel vm,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                vm.IsEdit = false;
                await LoadLookupDataAsync(vm, cancellationToken);
                return View(vm);
            }

            await _productService.CreateAdminProductAsync(vm.CreateDto, cancellationToken);

            TempData["Success"] = "Product created successfully.";
            return RedirectToAction("Index", "AdminProducts", new { area = "Adminn" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var product = await _productService.GetAdminProductByIdAsync(id, cancellationToken);
            if (product == null)
                return NotFound();

            var vm = new AdminProductFormViewModel
            {
                IsEdit = true,
                UpdateDto = new AdminUpdateProductDto
                {
                    Id = product.Id,
                    Title = product.Title,
                    Description = product.Description,
                    Price = product.Price,
                    DiscountPrice = product.DiscountPrice,
                    Stock = product.Stock,
                    IsActive = product.IsActive,
                    Color = product.Color,
                    Size = product.Size,
                    CategoryId = product.CategoryId,
                    BrandId = product.BrandId
                }
            };

            await LoadLookupDataAsync(vm, cancellationToken);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminProductFormViewModel vm,CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                vm.IsEdit = true;
                await LoadLookupDataAsync(vm, cancellationToken);
                return View(vm);
            }

            var updated = await _productService.UpdateAdminProductAsync(vm.UpdateDto, cancellationToken);
            if (!updated)
                return NotFound();

            TempData["Success"] = "Product updated successfully.";
            return RedirectToAction("Index", "AdminProducts", new { area = "Adminn" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var deleted = await _productService.DeleteAdminProductAsync(id, cancellationToken);

            TempData["Success"] = deleted
                ? "Product deleted successfully."
                : "Product not found.";

            return RedirectToAction("Index", "AdminProducts", new { area = "Adminn" });
        }

        private async Task LoadLookupDataAsync(AdminProductFormViewModel vm, CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);

            var brands = await _context.Brand
                .AsNoTracking()
                .Select(b => new AdminSelectItemViewModel
                {
                    Id = b.BrandID,
                    Name = b.BrandName
                })
                .ToListAsync(cancellationToken);

            vm.Categories = categories.Select(c => new AdminSelectItemViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            vm.Brands = brands;
        }
    }
}