using Mapster;
using Microsoft.EntityFrameworkCore;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.Interfaces.ICategoryService;
using ShopEgypt.Data.Context;
using ShopEgypt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}