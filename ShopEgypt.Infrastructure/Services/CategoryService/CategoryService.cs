using Mapster;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.Interfaces.ICategoryService;
using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;

namespace ShopEgypt.Infrastructure.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        public ApplicationDbContext Context { get; set; }

        public CategoryService(ApplicationDbContext context)
        {
            Context = context;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken)
        {
            var categories = await Context.Categories.ToListAsync(cancellationToken);
            return categories.Adapt<List<CategoryDto>>();
        }

        public async Task<List<Category>> GetAllCategoriesMainAsync(CancellationToken cancellationToken)
        {
            return await Context.Categories.ToListAsync(cancellationToken);
        }

        public async Task<List<CategoryAdminDto>> GetAllCategoriesForAdminAsync(CancellationToken cancellationToken)
        {
            return await Context.Categories
                .Select(c => new CategoryAdminDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    ProductCount = c.Products.Count()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<UpdateCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await Context.Categories
                .Where(c => c.Id == id)
                .Select(c => new UpdateCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl
            };

            await Context.Categories.AddAsync(category, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);

            return category.Id;
        }

        public async Task<bool> UpdateCategoryAsync(UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            var category = await Context.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.Id, cancellationToken);

            if (category == null)
                return false;

            category.Name = dto.Name;
            category.Description = dto.Description;
            category.ImageUrl = dto.ImageUrl;

            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
        {
            var category = await Context.Categories
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (category == null)
                return false;

            Context.Categories.Remove(category);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}