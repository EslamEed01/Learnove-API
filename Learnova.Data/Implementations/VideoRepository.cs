using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Implementations
{
    public class VideoRepository : IVideoRepository
    {

        private readonly LearnoveDbContext _learnoveDbContext;

        public VideoRepository(LearnoveDbContext learnoveDbContext)
        {
            _learnoveDbContext = learnoveDbContext;
        }

        public async Task<LessonVideo> AddAsync(LessonVideo video)
        {
            _learnoveDbContext.LessonVideos.Add(video);
            await _learnoveDbContext.SaveChangesAsync();
            return video;

        }

        public async Task<IEnumerable<LessonVideo>> GetByLessonIdAsync(int lessonId)
        {
            return await _learnoveDbContext.LessonVideos
           .Where(v => v.LessonId == lessonId)
           .ToListAsync();
        }
    }
}
