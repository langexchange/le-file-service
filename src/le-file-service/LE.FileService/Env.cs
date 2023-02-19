using System;

namespace LE.FileService
{
    public static class Env
    {
        public readonly static string S3_BUCKET_NAME = Environment.GetEnvironmentVariable("S3_BUCKET_NAME");
        public readonly static string AWS_ACCESS_KEY_ID = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        public readonly static string AWS_SECRET_ACCESS_KEY = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        public readonly static string MINIO_HOST = Environment.GetEnvironmentVariable("MINIO_HOST");
        public readonly static string APSoutheast1 = "ap-southeast-1";
    }
}
