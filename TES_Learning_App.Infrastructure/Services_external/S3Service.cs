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
        private readonly bool _useLocalStorage;
        private readonly string _localUploadsPath;

        public S3Service(IConfiguration config)
        {
            var region = config["AWS:Region"];
            var accessKey = config["AWS:AccessKeyId"];
            var secretKey = config["AWS:SecretAccessKey"];
            var environment = config["ASPNETCORE_ENVIRONMENT"] ?? config["Environment"] ?? "Development";
            var contentRootPath = config["ContentRootPath"] ?? AppDomain.CurrentDomain.BaseDirectory;

            // Check if we should use local storage (development without AWS credentials)
            _useLocalStorage = string.IsNullOrEmpty(accessKey) && string.IsNullOrEmpty(secretKey) && 
                              (environment == "Development" || environment == "Development");

            if (_useLocalStorage)
            {
                // Use local file storage for development
                _s3Client = null;
                _bucketName = null;
                _cloudFrontUrl = null;
                
                // Set up local uploads directory
                _localUploadsPath = Path.Combine(contentRootPath, "wwwroot", "uploads");
                if (!Directory.Exists(_localUploadsPath))
                {
                    Directory.CreateDirectory(_localUploadsPath);
                }
            }
            else
            {
                // Use AWS S3
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
                _localUploadsPath = null;
            }
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType = "application/octet-stream")
        {
            try
            {
                if (_useLocalStorage)
                {
                    // Save to local file system
                    var localFilePath = Path.Combine(_localUploadsPath, fileName);
                    
                    // Ensure directory exists
                    var directory = Path.GetDirectoryName(localFilePath);
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Save file
                    using (var fileStreamLocal = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await fileStream.CopyToAsync(fileStreamLocal);
                    }

                    // Return local URL path
                    return $"/uploads/{fileName}";
                }
                else
                {
                    // Upload to S3
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
            }
            catch (Exception ex)
            {
                if (_useLocalStorage)
                {
                    throw new Exception($"Error uploading file to local storage: {ex.Message}", ex);
                }
                else
                {
                    throw new Exception($"Error uploading file to S3: {ex.Message}", ex);
                }
            }
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            try
            {
                if (_useLocalStorage)
                {
                    // Remove /uploads/ prefix if present
                    var cleanFileName = fileName;
                    if (cleanFileName.StartsWith("/uploads/"))
                    {
                        cleanFileName = cleanFileName.Substring("/uploads/".Length);
                    }
                    else if (cleanFileName.StartsWith("uploads/"))
                    {
                        cleanFileName = cleanFileName.Substring("uploads/".Length);
                    }

                    var localFilePath = Path.Combine(_localUploadsPath, cleanFileName);
                    if (File.Exists(localFilePath))
                    {
                        File.Delete(localFilePath);
                    }
                    return true;
                }
                else
                {
                    // Remove /uploads/ prefix if present (legacy local files)
                    var s3Key = fileName;
                    if (s3Key.StartsWith("/uploads/"))
                    {
                        s3Key = s3Key.Substring("/uploads/".Length);
                    }
                    else if (s3Key.StartsWith("uploads/"))
                    {
                        s3Key = s3Key.Substring("uploads/".Length);
                    }

                    var request = new DeleteObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = s3Key
                    };

                    await _s3Client.DeleteObjectAsync(request);
                    return true;
                }
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

            // If it starts with /uploads, it's a local file path - return as is
            if (fileName.StartsWith("/uploads/"))
                return fileName;

            if (_useLocalStorage)
            {
                // For local storage, ensure it starts with /uploads/
                if (fileName.StartsWith("uploads/"))
                {
                    return $"/{fileName}";
                }
                return $"/uploads/{fileName}";
            }
            else
            {
                // For S3, remove /uploads/ prefix if present (legacy local files)
                var s3Key = fileName;
                if (s3Key.StartsWith("/uploads/"))
                {
                    s3Key = s3Key.Substring("/uploads/".Length);
                }
                else if (s3Key.StartsWith("uploads/"))
                {
                    s3Key = s3Key.Substring("uploads/".Length);
                }

                // Return CloudFront URL if configured
                if (!string.IsNullOrEmpty(_cloudFrontUrl))
                {
                    return $"{_cloudFrontUrl}/{s3Key}";
                }

                // Fallback to S3 URL
                var region = _s3Client.Config.RegionEndpoint.SystemName;
                return $"https://{_bucketName}.s3.{region}.amazonaws.com/{s3Key}";
            }
        }
    }
}

