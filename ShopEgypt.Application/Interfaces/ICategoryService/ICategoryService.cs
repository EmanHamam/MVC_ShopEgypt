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
    }
}
