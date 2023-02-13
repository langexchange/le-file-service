using LE.FileService.MinIO;
using LE.FileService.S3;
using LE.FileService.Services;
using LE.Library.LE.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace LE.FileService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LE.FileService", Version = "v1" });
            });
            services.AddScoped<IFileService, Services.FileService>();
            //services.AddMinIOStorage(sp => new MinIOConfig
            //{
            //    AwsBucket = Env.S3_BUCKET_NAME,
            //    AwsKeyId = Env.AWS_ACCESS_KEY_ID,
            //    AwsSecretKey = Env.AWS_SECRET_ACCESS_KEY,
            //    RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1,
            //    ACL = Amazon.S3.S3CannedACL.Private,
            //    AmazonS3Config = new Amazon.S3.AmazonS3Config
            //    {
            //        AuthenticationRegion = Amazon.RegionEndpoint.USEast1.SystemName,// Should match the `MINIO_REGION`         
            //                                                                        //AuthenticationRegion = "",
            //        ServiceURL = Env.MINIO_HOST,
            //        ForcePathStyle = true// MUST be true to work correctly with MinIO server}
            //    }
            //});
            services.AddSS3torage(sp => new S3Config
            {
                AwsBucket = Env.S3_BUCKET_NAME,
                AwsKeyId = Env.AWS_ACCESS_KEY_ID,
                AwsSecretKey = Env.AWS_SECRET_ACCESS_KEY,
                RegionEndpoint = Amazon.RegionEndpoint.APSoutheast1,
                ACL = Amazon.S3.S3CannedACL.Private,
            });
            services.AddHealthChecks();
            services.AddConsul();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LE.FileService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseConsul();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
