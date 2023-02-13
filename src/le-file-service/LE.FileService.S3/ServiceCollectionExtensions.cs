using Amazon.S3;
using LE.FileService.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LE.FileService.S3
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSS3torage(this IServiceCollection services, Func<IServiceProvider, S3Config> config)
        {
            services.AddScoped<IStorage>(sp =>
            {
                var storageConfig = config(sp);

                var s3Client = new AmazonS3Client(storageConfig.AwsKeyId, storageConfig.AwsSecretKey, storageConfig.RegionEndpoint);

                return new S3Storage(s3Client, storageConfig);
            });
            return services;
        }
    }
}
