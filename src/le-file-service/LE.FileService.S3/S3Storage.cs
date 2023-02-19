using Amazon.S3;
using Amazon.S3.Model;
using LE.FileService.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LE.FileService.S3
{
    public class S3Storage : IStorage
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly S3Config _s3Config;

        public S3Storage(IAmazonS3 amazonS3, S3Config s3Config)
        {
            _amazonS3 = amazonS3;
            _s3Config = s3Config;
        }
        public Task<string> GetDownloadUrlFromStreamIdAsync(StorageInfo streamInfo, CancellationToken cancellationToken = default)
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = string.IsNullOrWhiteSpace(streamInfo.Root) ? _s3Config.AwsBucket : streamInfo.Root,
                Key = streamInfo.StreamId,
                Expires = DateTime.UtcNow.AddMinutes(10),
                Verb = HttpVerb.GET,
                Protocol = Protocol.HTTPS
            };
            return Task.FromResult(_amazonS3.GetPreSignedURL(request));
        }
        public async Task<string> StoreObjectStreamAsync(StorageInfo streamInfo, string contentType, Stream stream, CancellationToken cancellationToken = default)
        {
            if (streamInfo.IsTransient())
                return null;

            var request = new PutObjectRequest
            {
                BucketName = string.IsNullOrWhiteSpace(streamInfo.Root) ? _s3Config.AwsBucket : streamInfo.Root,
                Key = streamInfo.StreamId,
                InputStream = stream,
                CannedACL = _s3Config.ACL,
                ContentType = contentType,
            };
            //var contentDisposition = MimeTypes.Image.SvgXml == contentType ? $"attachment; filename=\"{streamInfo.StreamId}\"" : string.Empty;

            //request.Headers.ContentDisposition = contentDisposition;
            request.Headers.ContentDisposition = string.Empty;

            var response = await _amazonS3.PutObjectAsync(request, cancellationToken);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK ? streamInfo.StreamId : null;
        }
    }
}
