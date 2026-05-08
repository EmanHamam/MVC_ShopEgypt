using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ShopEgypt.Application.DTOs;

namespace ShopEgypt.Application.Interfaces.IImageStorageService
{
    public interface IImageStorageService
    {
        Task<ImageUploadResultDto> UploadAsync(
            Stream stream,
            string fileName,
            string contentType,
            string? folder,
            CancellationToken cancellationToken
        );

    }
}
