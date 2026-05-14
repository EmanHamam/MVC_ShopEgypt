using ShopEgypt.Application.DTOs;
using ShopEgypt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.Interfaces.ICategoryService
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken);
        Task<List<Category>> GetAllCategoriesMainAsync(CancellationToken cancellationToken);

        //for admin
        Task<List<CategoryAdminDto>> GetAllCategoriesForAdminAsync(CancellationToken cancellationToken);
        Task<UpdateCategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
        Task<int> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateCategoryAsync(UpdateCategoryDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken);
    }
}
