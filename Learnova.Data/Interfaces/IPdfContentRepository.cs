using Learnova.Domain.Entities;

namespace Learnova.Infrastructure.Interfaces
{
    public interface IPdfContentRepository
    {
        Task<pdfContents> AddAsync(pdfContents pdfContent);
        Task<pdfContents?> GetByCourseIdAsync(int courseId);
        Task<pdfContents?> GetByIdAsync(Guid id);
    }
}
