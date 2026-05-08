using System.Threading;
using System.Threading.Tasks;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Domain.Entities;

namespace ShopEgypt.Application.Interfaces.IProductService
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductListItemDto>> GetAllProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<ProductDetailDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
        Task<Product?> GetProductEntityByIdAsync(int id, CancellationToken cancellationToken);

        Task<Product> CreateProductAsync(Product product, CancellationToken cancellationToken);

        Task<Product> UpdateProductAsync(Product product, CancellationToken cancellationToken);

        Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken);

        Task<int> GetNextImageOrderAsync(int productId, CancellationToken cancellationToken);
    }
}
