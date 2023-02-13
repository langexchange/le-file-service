using Amazon.S3;
using LE.FileService.S3;
using LE.FileService.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LE.FileService.MinIO
{
    public static class ServiceCollectionExtensions
    {
         public static IServiceCollection AddMinIOStorage(this IServiceCollection services, Func<IServiceProvider, MinIOConfig> config)
        {
            services.AddScoped<IStorage>(sp =>
            {
                var storageConfig = config(sp);

                var minIOClient = new AmazonS3Client(storageConfig.AwsKeyId, storageConfig.AwsSecretKey, storageConfig.AmazonS3Config);

                return new MinIOStorage(minIOClient, storageConfig);
            });
            return services;
        }
    }
}
