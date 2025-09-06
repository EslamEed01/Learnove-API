using Amazon.S3;
using Amazon.S3.Model;
using Learnova.Business.Services.Interfaces;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Learnova.Business.Implementations
{
    public class PdfUploadService : IPdfUploadService
    {
        private readonly IPdfContentRepository _pdfContentRepository;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB

        public PdfUploadService(IPdfContentRepository pdfContentRepository, IAmazonS3 s3Client, IConfiguration configuration)
        {
            _pdfContentRepository = pdfContentRepository;
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _bucketName = configuration["AWS:S3:BucketName"];
        }

        public async Task<pdfContents> UploadPdfAsync(int courseId, IFormFile file, string uploadedById)
        {
            ValidatePdfFile(file);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var key = $"pdfs/{courseId}/{fileName}";

            try
            {
                // Upload to S3
                var uploadRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = file.OpenReadStream(),
                    ContentType = "application/pdf",
                    ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
                };

                var response = await _s3Client.PutObjectAsync(uploadRequest);

                var pdfContent = new pdfContents
                {
                    CourseId = courseId,
                    FileName = fileName,
                    FileUrl = key,
                    FileSize = file.Length,
                    CreatedAt = DateTime.UtcNow,
                    S3FileUrl = $"https://{_bucketName}.s3.amazonaws.com/{key}",
                    UploadedById = uploadedById
                };

                await _pdfContentRepository.AddAsync(pdfContent);

                return pdfContent;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(message: $"Failed to upload PDF: {ex.Message}", ex);
            }
        }

        private void ValidatePdfFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty.");

            if (!Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("File must have a .pdf extension.");

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only PDF files are allowed.");

            if (file.Length > _maxFileSize)
                throw new ArgumentException($"File size exceeds maximum allowed size of {_maxFileSize / (1024 * 1024)}MB.");

            using var stream = file.OpenReadStream();
            var buffer = new byte[4];
            stream.Read(buffer, 0, 4);

            if (buffer[0] != 0x25 || buffer[1] != 0x50 || buffer[2] != 0x44 || buffer[3] != 0x46)
                throw new ArgumentException("Invalid PDF file format.");
        }
    }
}