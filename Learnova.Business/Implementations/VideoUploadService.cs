using Amazon.S3;
using Amazon.S3.Model;
using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Learnova.Business.Implementations
{
    public class VideoUploadService : IVideoUploadService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly long _maxFileSize = 40 * 1024 * 1024; // 40MB

        public VideoUploadService(IVideoRepository videoRepository, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _videoRepository = videoRepository;
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _bucketName = configuration["AWS:S3:BucketName"];
        }

        public async Task<LessonVideo> UploadVideoAsync(int lessonId, IFormFile file, string uploadedById)
        {
            ValidateVideoFile(file);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var key = $"videos/{lessonId}/{fileName}";

            try
            {
                var uploadRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = file.OpenReadStream(),
                    ContentType = "video/mp4",
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
                };

                await _s3Client.PutObjectAsync(uploadRequest);

                var videoContent = new LessonVideo
                {
                    LessonId = lessonId,
                    FileName = file.FileName,
                    FileUrl = key,
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    S3FileUrl = $"https://{_bucketName}.s3.amazonaws.com/{key}",
                    UploadedById = uploadedById
                };

                await _videoRepository.AddAsync(videoContent);

                return videoContent;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to upload video: {ex.Message}", ex);
            }
        }

        private void ValidateVideoFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty.");

            if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".mp4")
                throw new ArgumentException("Only MP4 video files are allowed.");

            if (file.Length > _maxFileSize)
                throw new ArgumentException($"File size exceeds maximum allowed size of {_maxFileSize / (1024 * 1024)}MB.");

            if (file.ContentType != "video/mp4")
                throw new ArgumentException("Invalid MIME type. Only 'video/mp4' is allowed.");
        }
    }
}

