using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface IVideoRepository
    {
        Task<LessonVideo> AddAsync(LessonVideo video);
        Task<IEnumerable<LessonVideo>> GetByLessonIdAsync(int lessonId);

    }
}
