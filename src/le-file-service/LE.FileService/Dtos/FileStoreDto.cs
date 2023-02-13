using System;

namespace LE.FileService.Dtos
{
    public class FileStoreDto
    {
        public string StreamId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}