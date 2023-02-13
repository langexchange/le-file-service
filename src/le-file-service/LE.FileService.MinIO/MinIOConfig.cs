using Amazon.S3;
using LE.FileService.S3;

namespace LE.FileService.MinIO
{
    public class MinIOConfig : S3Config
    {
        public AmazonS3Config AmazonS3Config { get; set; }
    }
}
