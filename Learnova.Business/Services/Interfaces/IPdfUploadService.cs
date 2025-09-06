using Learnova.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Learnova.Business.Services.Interfaces
{
    public interface IPdfUploadService
    {
        Task<pdfContents> UploadPdfAsync(int courseId, IFormFile file, string uploadedById);
    }
}
