using LE.FileService.Dtos;
using LE.FileService.Helpers;
using LE.FileService.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LE.FileService.Services
{
    public class FileService : IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly IStorage _storage;

        public FileService(ILogger<FileService> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public Task<string> GetDownloadUrlFromStreamIdAsync(string streamIdValue, string root = null, CancellationToken cancellationToken = default)
        {
            var streamInfo = new StorageInfo
            {
                StreamId = streamIdValue,
                Root = root
            };
            return _storage.GetDownloadUrlFromStreamIdAsync(streamInfo, cancellationToken);
        }

        public async Task<FileStoreDto> UploadToPathAsync(IFormFile file, string path, string root = null, CancellationToken cancellationToken = default)
        {
            var streamIdBuilder = new StringBuilder();

            var fileName = $"{file.FileName}";

            streamIdBuilder.Append(!string.IsNullOrWhiteSpace(path) ? path.TrimEnd('/') : "").Append($"/{fileName}");

            var contentType = file.ContentType;

            var stream = new MemoryStream();

            //var isImage = UploadConfigValue.ImageMimeTypes.Contains(contentType);
            //if (isImage)
            //{
            //    var image = file.Resize(UploadConfigValue.MAX_IMG_HEIGHT);
            //    image.SaveAsPng(stream);
            //}
            //else
            //{
            //    file.CopyTo(stream);
            //}
            file.CopyTo(stream);
            _logger.LogInformation("UploadToPathAsync {path}, streamId: {streamId}", path, streamIdBuilder.ToString());

            var fileSize = stream.Length;

            var streamInfo = new StorageInfo
            {
                StreamId = streamIdBuilder.ToString(),
                Root = root
            };

            var streamId = await _storage.StoreObjectStreamAsync(streamInfo, contentType, stream, cancellationToken: cancellationToken);

            return new FileStoreDto
            {
                ContentType = contentType,
                FileName = fileName,
                Name = file.Name,
                FileSize = fileSize,
                StreamId = streamId,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public Task<FileStoreDto> UploadAsync(int id, IFormFile file, string root = null, CancellationToken cancellationToken = default)
        {
            var xPath = FileHelper.GenFullPath(id);

            return UploadToPathAsync(file, xPath, root, cancellationToken);
        }
    }
}
