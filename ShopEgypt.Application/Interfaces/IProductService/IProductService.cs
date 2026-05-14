using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.DTOs.Admin;
using ShopEgypt.Domain.Entities;
using ShopEgypt.Domain.Enums.ProductEnums;
using System.Threading;
using System.Threading.Tasks;

namespace ShopEgypt.Application.Interfaces.IProductService
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductListItemDto>> GetAllProductsAsync(int pageNumber, int pageSize,int? categoryId ,
            ProductSortBy? sortBy,string? keyWord, decimal? minPrice, decimal? maxPrice, CancellationToken cancellationToken);
        Task<ProductDetailDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
        Task<Product?> GetProductEntityByIdAsync(int id, CancellationToken cancellationToken);

        Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken);

        Task<Product> UpdateProductAsync(Product product, CancellationToken cancellationToken);

        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken);

        Task<int> GetNextImageOrderAsync(int productId, CancellationToken cancellationToken);


        Task<PagedResultDto<AdminProductListItemDto>> GetAdminProductsAsync(
            AdminProductFilterDto filter,
            CancellationToken cancellationToken);

        Task<AdminProductDetailsDto?> GetAdminProductByIdAsync(int id, CancellationToken cancellationToken);
        Task<int> CreateAdminProductAsync(AdminCreateProductDto dto, CancellationToken cancellationToken);
        Task<bool> UpdateAdminProductAsync(AdminUpdateProductDto dto, CancellationToken cancellationToken);
        Task<bool> DeleteAdminProductAsync(int id, CancellationToken cancellationToken);
    }
}
