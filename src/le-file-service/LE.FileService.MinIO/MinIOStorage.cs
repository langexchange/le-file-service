using Amazon.S3;
using LE.FileService.S3;

namespace LE.FileService.MinIO
{
    public class MinIOStorage : S3Storage
    {
        public MinIOStorage(IAmazonS3 amazonS3, S3Config s3Config) : base(amazonS3, s3Config)
        {
        }
    }
}
