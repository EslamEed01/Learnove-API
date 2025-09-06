using Learnova.Business.DTOs.PdfProcessingDTO;
using Learnova.Business.Services.Interfaces.PdfProccessingServices;
using Learnova.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfProcessingController : ControllerBase
    {
        private readonly IPdfProcessingService _pdfProcessingService;
        private readonly IPdfContentRepository _pdfContentRepository;

        public PdfProcessingController(IPdfProcessingService pdfProcessingService, IPdfContentRepository pdfContentRepository)
        {
            _pdfProcessingService = pdfProcessingService;
            _pdfContentRepository = pdfContentRepository;
        }

        /// <summary>
        /// Process a PDF document by its content ID.  
        /// </summary>
        [HttpPost("process/{pdfContentId}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessPdf(Guid pdfContentId, [FromBody] PdfProcessingOptions? options = null)
        {
            var pdfContent = await _pdfContentRepository.GetByIdAsync(pdfContentId);
            if (pdfContent == null)
            {
                return NotFound($"PDF content with ID {pdfContentId} not found.");
            }

            var s3Key = pdfContent.FileUrl;

            var result = await _pdfProcessingService.ProcessPdfAsync(pdfContentId, "learnovaapis3", s3Key, options);
            return Ok(result);
        }

        /// <summary>
        /// Delete all chunks associated with a specific PDF content ID.  
        /// </summary>
        [HttpDelete("chunks/{pdfContentId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePdfChunks(Guid pdfContentId)
        {
            var result = await _pdfProcessingService.DeletePdfChunksAsync(pdfContentId);
            return Ok(result);
        }

        /// <summary>
        /// Get a summary of the processing status for a specific PDF content ID.  
        /// </summary>
        [HttpGet("summary/{pdfContentId}")]
        [Authorize(Roles = "Admin,instructor")]
        public async Task<IActionResult> GetProcessingSummary(Guid pdfContentId)
        {
            var summary = await _pdfProcessingService.GetProcessingSummaryAsync(pdfContentId);
            return Ok(summary);
        }
    }
}
