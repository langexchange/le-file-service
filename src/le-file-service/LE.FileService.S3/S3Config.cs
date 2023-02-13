using Amazon;
using Amazon.S3;

namespace LE.FileService.S3
{
    public class S3Config
    {
        public string AwsKeyId { get; set; }
        public string AwsSecretKey { get; set; }
        public string AwsBucket { get; set; }
        public string AwsS3Path { get; set; }

        public RegionEndpoint RegionEndpoint { get; set; }
        public S3CannedACL ACL { get; set; }
    }
}
