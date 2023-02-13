using LE.FileService.Dtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LE.FileService.Services
{
    public interface IFileService
    {
        Task<FileStoreDto> UploadAsync(int id, IFormFile file, string root = null, CancellationToken cancellationToken = default);

        Task<FileStoreDto> UploadToPathAsync(IFormFile file, string path, string root = null, CancellationToken cancellationToken = default);
        Task<string> GetDownloadUrlFromStreamIdAsync(string streamIdValue, string root = null, CancellationToken cancellationToken = default);
    }
}
