using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Learnova.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Learnova.Infrastructure.Implementations
{
    public class PdfContentRepository : IPdfContentRepository
    {
        private readonly LearnoveDbContext _learnoveDbContext;

        public PdfContentRepository(LearnoveDbContext learnoveDbContext)
        {
            _learnoveDbContext = learnoveDbContext;
        }

        public async Task<pdfContents> AddAsync(pdfContents pdfContent)
        {
            _learnoveDbContext.pdfContents.Add(pdfContent);
            await _learnoveDbContext.SaveChangesAsync();
            return pdfContent;
        }

        public async Task<pdfContents?> GetByCourseIdAsync(int courseId)
        {
            return await _learnoveDbContext.pdfContents
           .FirstOrDefaultAsync(p => p.CourseId == courseId);
        }

        public async Task<pdfContents?> GetByIdAsync(Guid id)
        {
            return await _learnoveDbContext.pdfContents
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
