using Learnova.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Learnova.Business.Services.Interfaces
{
    public interface IVideoUploadService
    {
        Task<LessonVideo> UploadVideoAsync(int lessonId, IFormFile file, string uploadedById);
    }
}
