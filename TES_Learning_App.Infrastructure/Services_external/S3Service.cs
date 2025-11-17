using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.Interfaces.Infrastructure;

namespace TES_Learning_App.Infrastructure.Services_external
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _cloudFrontUrl;

        public S3Service(IConfiguration config)
        {
            var region = config["AWS:Region"];
            var accessKey = config["AWS:AccessKeyId"];
            var secretKey = config["AWS:SecretAccessKey"];

            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region ?? "ap-southeast-1");

            if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            {
                _s3Client = new AmazonS3Client(accessKey, secretKey, regionEndpoint);
            }
            else
            {
                // Use default credentials (AWS profile, IAM role, etc.)
                _s3Client = new AmazonS3Client(regionEndpoint);
            }

            _bucketName = config["AWS:BucketName"] ?? throw new InvalidOperationException("AWS:BucketName is required in configuration");
            _cloudFrontUrl = config["AWS:CloudFrontUrl"]?.TrimEnd('/') ?? "";
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType = "application/octet-stream")
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    InputStream = fileStream,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.PublicRead // Make files publicly accessible
                };

                await _s3Client.PutObjectAsync(request);

                // Return the CloudFront URL if configured, otherwise return S3 URL
                if (!string.IsNullOrEmpty(_cloudFrontUrl))
                {
                    return $"{_cloudFrontUrl}/{fileName}";
                }

                // Fallback to S3 URL
                var region = _s3Client.Config.RegionEndpoint.SystemName;
                return $"https://{_bucketName}.s3.{region}.amazonaws.com/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file to S3: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName
                };

                await _s3Client.DeleteObjectAsync(request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetFileUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            // If it's already a full URL (including CloudFront or S3), return as is
            if (fileName.StartsWith("http://") || fileName.StartsWith("https://"))
                return fileName;

            // If it starts with /uploads, it's a legacy local file path
            // Convert /uploads/profiles/xxx.jpg to profiles/xxx.jpg for S3
            if (fileName.StartsWith("/uploads/"))
            {
                // Remove the leading /uploads/ prefix
                fileName = fileName.Substring("/uploads/".Length);
            }
            else if (fileName.StartsWith("uploads/"))
            {
                // Remove the leading uploads/ prefix if present
                fileName = fileName.Substring("uploads/".Length);
            }

            // Return CloudFront URL if configured
            if (!string.IsNullOrEmpty(_cloudFrontUrl))
            {
                return $"{_cloudFrontUrl}/{fileName}";
            }

            // Fallback to S3 URL
            var region = _s3Client.Config.RegionEndpoint.SystemName;
            return $"https://{_bucketName}.s3.{region}.amazonaws.com/{fileName}";
        }
    }
}

