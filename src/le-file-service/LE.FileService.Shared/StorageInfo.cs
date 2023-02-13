using System;

namespace LE.FileService.Shared
{
    public class StorageInfo
    {
        public string StreamId { get; set; }
        public string Path { get; set; }
        public string Root { get; set; }
        public DateTime LastModified { get; set; }

        public bool IsTransient() => string.IsNullOrWhiteSpace(StreamId);
    }
}
