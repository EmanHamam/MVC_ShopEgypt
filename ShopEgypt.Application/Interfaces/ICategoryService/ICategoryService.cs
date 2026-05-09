using ShopEgypt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShopEgypt.Application.Interfaces.ICategoryService
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken);

    }
}
