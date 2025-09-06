using Learnova.Business.DTOs.AiDTO;
using Learnova.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Learnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<AIController> _logger;

        public AIController(IAIService aiService, ILogger<AIController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }


        /// <summary>
        /// ask a question about the uploaded PDF documents
        /// </summary>

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] AIQueryRequest request)
        {
            var requestId = Guid.NewGuid();
            _logger.LogInformation("Processing AI question request {RequestId}: {Question}", requestId, request.Question);

            if (string.IsNullOrWhiteSpace(request.Question))
            {
                _logger.LogWarning("AI question request {RequestId} failed: Question is required", requestId);
                return BadRequest(new
                {
                    Error = "Question is required",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }

            try
            {
                var response = await _aiService.AskQuestionAsync(request);

                if (response.Success)
                {
                    _logger.LogInformation("AI question request {RequestId} completed successfully. " +
                        "Chunks found: {TotalChunks}, Chunks used: {UsedChunks}, Processing time: {ProcessingTime}ms",
                        requestId,
                        response.Metadata?.TotalChunksFound ?? 0,
                        response.Metadata?.ChunksUsed ?? 0,
                        response.ProcessingTime.TotalMilliseconds);
                }
                else
                {
                    _logger.LogWarning("AI question request {RequestId} failed: {Error}", requestId, response.ErrorMessage);
                }

                return Ok(new
                {
                    RequestId = requestId,
                    Success = response.Success,
                    Answer = response.Answer,
                    ErrorMessage = response.ErrorMessage,
                    RetrievedChunks = response.RetrievedChunks?.Take(5).Select(c => new
                    {
                        ChunkId = c.ChunkId,
                        Content = c.Content.Length > 200 ? c.Content.Substring(0, 200) + "..." : c.Content,
                        SimilarityScore = c.SimilarityScore,
                        PageNumbers = c.PageNumbers,
                        ChunkIndex = c.ChunkIndex
                    }),
                    Metadata = response.Metadata,
                    ProcessingTime = $"{response.ProcessingTime.TotalMilliseconds:F0}ms",
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing AI question request {RequestId}: {Question}", requestId, request.Question);
                return StatusCode(500, new
                {
                    Error = "Internal server error occurred while processing your question",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Generate a summary of a PDF document
        /// </summary>
        [HttpPost("summarize")]
        public async Task<IActionResult> SummarizePdf([FromBody] PdfSummaryRequest request)
        {
            var requestId = Guid.NewGuid();
            _logger.LogInformation("Processing PDF summary request {RequestId} for PDF: {PdfId}, Type: {SummaryType}, Length: {SummaryLength}",
                requestId, request.PdfContentId, request.Type, request.Length);

            if (request.PdfContentId == Guid.Empty)
            {
                _logger.LogWarning("PDF summary request {RequestId} failed: PDF Content ID is required", requestId);
                return BadRequest(new
                {
                    Error = "PDF Content ID is required",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }

            try
            {
                var response = await _aiService.SummarizePdfAsync(request);

                if (response.Success)
                {
                    _logger.LogInformation("PDF summary request {RequestId} completed successfully. " +
                        "Chunks processed: {ChunkCount}, Processing time: {ProcessingTime}ms",
                        requestId,
                        response.Metadata?.ChunksUsed ?? 0,
                        response.ProcessingTime.TotalMilliseconds);
                }
                else
                {
                    _logger.LogWarning("PDF summary request {RequestId} failed: {Error}", requestId, response.ErrorMessage);
                }

                return Ok(new
                {
                    RequestId = requestId,
                    Success = response.Success,
                    Summary = response.Answer,
                    ErrorMessage = response.ErrorMessage,
                    ChunksProcessed = response.RetrievedChunks?.Count ?? 0,
                    Metadata = response.Metadata,
                    ProcessingTime = $"{response.ProcessingTime.TotalMilliseconds:F0}ms",
                    SummaryType = request.Type,
                    SummaryLength = request.Length,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PDF summary request {RequestId} for PDF: {PdfId}", requestId, request.PdfContentId);
                return StatusCode(500, new
                {
                    Error = "Internal server error occurred while generating summary",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Search for similar content chunks using vector similarity
        /// </summary>
        [HttpPost("search")]
        public async Task<IActionResult> SearchSimilar([FromBody] SearchRequest request)
        {
            var requestId = Guid.NewGuid();
            _logger.LogInformation("Processing similarity search request {RequestId}: Query='{Query}', PDF={PdfId}, MaxResults={MaxResults}",
                requestId, request.Query, request.PdfContentId, request.MaxResults);

            if (string.IsNullOrWhiteSpace(request.Query))
            {
                _logger.LogWarning("Similarity search request {RequestId} failed: Search query is required", requestId);
                return BadRequest(new
                {
                    Error = "Search query is required",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }

            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var chunks = await _aiService.SearchSimilarChunksAsync(
                    request.Query,
                    request.PdfContentId,
                    request.MaxResults);
                stopwatch.Stop();

                _logger.LogInformation("Similarity search request {RequestId} completed. Found {ChunkCount} chunks in {ProcessingTime}ms",
                    requestId, chunks.Count, stopwatch.ElapsedMilliseconds);

                if (chunks.Any())
                {
                    _logger.LogInformation("📊 Found chunks with scores: {Scores}",
                        string.Join(", ", chunks.Select(c => $"{c.ChunkId.ToString()[..8]}:{c.SimilarityScore:F3}")));
                }

                return Ok(new
                {
                    RequestId = requestId,
                    Success = true,
                    Chunks = chunks.Select(c => new
                    {
                        ChunkId = c.ChunkId,
                        Content = c.Content.Length > 300 ? c.Content.Substring(0, 300) + "..." : c.Content,
                        SimilarityScore = c.SimilarityScore,
                        PageNumbers = c.PageNumbers,
                        ChunkIndex = c.ChunkIndex,
                        PdfContentId = c.PdfContentId
                    }),
                    TotalFound = chunks.Count,
                    ProcessingTime = $"{stopwatch.ElapsedMilliseconds}ms",
                    Message = chunks.Any() ? $"Found {chunks.Count} relevant chunks" : "No relevant content found",
                    SearchMetadata = new
                    {
                        Query = request.Query,
                        QueryLength = request.Query.Length,
                        PdfFilter = request.PdfContentId.HasValue ? "Applied" : "None",
                        SearchMethod = "Cohere Embeddings + Pinecone Vector Search",
                        SimilarityScores = chunks.Take(5).Select(c => new
                        {
                            ChunkId = c.ChunkId.ToString()[..8],
                            Score = c.SimilarityScore
                        }).ToArray()
                    },
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing similarity search request {RequestId}: {Query}", requestId, request.Query);
                return StatusCode(500, new
                {
                    Error = "Internal server error occurred during search",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Get chunks from a specific PDF (for debugging/testing)
        /// </summary>
        [HttpGet("chunks/{pdfContentId}")]
        public async Task<IActionResult> GetPdfChunks(Guid pdfContentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var requestId = Guid.NewGuid();
            _logger.LogInformation("Getting PDF chunks request {RequestId} for PDF: {PdfId}, Page: {Page}, PageSize: {PageSize}",
                requestId, pdfContentId, page, pageSize);

            if (pdfContentId == Guid.Empty)
            {
                return BadRequest(new
                {
                    Error = "Valid PDF Content ID is required",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (page < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new
                {
                    Error = "Page must be >= 1 and PageSize must be between 1 and 100",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }

            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var chunks = await _aiService.SearchSimilarChunksAsync(
                    "document content analysis overview",
                    pdfContentId,
                    pageSize * 10);
                stopwatch.Stop();

                var totalChunks = chunks.Count;
                var pagedChunks = chunks
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("PDF chunks request {RequestId} completed. Total: {TotalChunks}, Returned: {ReturnedChunks}, Processing time: {ProcessingTime}ms",
                    requestId, totalChunks, pagedChunks.Count, stopwatch.ElapsedMilliseconds);

                return Ok(new
                {
                    RequestId = requestId,
                    Success = true,
                    Chunks = pagedChunks.Select(c => new
                    {
                        ChunkId = c.ChunkId,
                        Content = c.Content,
                        SimilarityScore = c.SimilarityScore,
                        PageNumbers = c.PageNumbers,
                        ChunkIndex = c.ChunkIndex,
                        ContentLength = c.Content.Length
                    }),
                    Pagination = new
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalChunks = totalChunks,
                        TotalPages = (int)Math.Ceiling((double)totalChunks / pageSize),
                        HasNextPage = (page * pageSize) < totalChunks,
                        HasPreviousPage = page > 1
                    },
                    ProcessingTime = $"{stopwatch.ElapsedMilliseconds}ms",
                    PdfContentId = pdfContentId,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PDF chunks request {RequestId} for PDF: {PdfId}", requestId, pdfContentId);
                return StatusCode(500, new
                {
                    Error = "Internal server error occurred while retrieving chunks",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        public class SearchRequest
        {
            public string Query { get; set; } = string.Empty;
            public Guid? PdfContentId { get; set; }
            public int MaxResults { get; set; } = 5;
        }

    }


}




