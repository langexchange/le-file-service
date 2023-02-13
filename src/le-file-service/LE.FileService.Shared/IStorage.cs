using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LE.FileService.Shared
{
    public interface IStorage
    {
        Task<string> StoreObjectStreamAsync(StorageInfo streamInfo, string contentType, Stream stream, CancellationToken cancellationToken = default);
        Task<string> GetDownloadUrlFromStreamIdAsync(StorageInfo streamInfo, CancellationToken cancellationToken = default);
    }
}
