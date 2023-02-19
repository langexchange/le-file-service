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
        public string FileType { get; set; }
        public string Url => $"https://{Env.S3_BUCKET_NAME}.s3.{Env.APSoutheast1}.amazonaws.com/{StreamId}";
    }
}