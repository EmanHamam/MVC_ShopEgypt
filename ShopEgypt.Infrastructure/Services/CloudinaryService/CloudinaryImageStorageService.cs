using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using ShopEgypt.Application.DTOs;
using ShopEgypt.Application.Interfaces.IImageStorageService;

namespace ShopEgypt.Infrastructure.Services.CloudinaryService
{
    public class CloudinaryImageStorageService : IImageStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryImageStorageService(IOptions<CloudinarySettings> options)
        {
            var settings = options.Value;
            _cloudinary = new Cloudinary(new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            ));
        }

        public async Task<ImageUploadResultDto> UploadAsync(
            Stream stream,
            string fileName,
            string contentType,
            string? folder,
            CancellationToken cancellationToken
        )
        {
            if (stream == null || stream.Length == 0)
            {
                throw new InvalidOperationException("Empty upload stream.");
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
            if (result.Error != null)
            {
                throw new InvalidOperationException(result.Error.Message);
            }

            return new ImageUploadResultDto
            {
                Url = result.SecureUrl?.ToString() ?? string.Empty,
                PublicId = result.PublicId
            };
        }
    }
}
