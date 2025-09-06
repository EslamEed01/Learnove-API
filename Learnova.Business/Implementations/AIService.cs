using Learnova.Business.DTOs.AiDTO;
using Learnova.Business.Services.Interfaces;
using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Learnova.Domain.Enums;
using Learnova.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Learnova.Business.Implementations
{
    public class AIService : IAIService
    {
        private readonly IPineconeService _pineconeService;
        private readonly LearnoveDbContext _dbContext;
        private readonly ILogger<AIService> _logger;
        private readonly IEmbeddingService _embeddingService;
        private readonly IGeminiService _geminiService;

        public AIService(
            IPineconeService pineconeService,
            LearnoveDbContext dbContext,
            IEmbeddingService embeddingService,
            IGeminiService geminiService,
            ILogger<AIService> logger)
        {
            _pineconeService = pineconeService;
            _dbContext = dbContext;
            _embeddingService = embeddingService;
            _geminiService = geminiService;
            _logger = logger;
        }

        public async Task<List<RetrievedChunk>> SearchSimilarChunksAsync(
            string query,
            Guid? pdfContentId = null,
            int maxResults = 5)
        {
            try
            {
                _logger.LogInformation(" Starting similarity search for query: '{Query}', PDF: {PdfId}, MaxResults: {MaxResults}",
                    query, pdfContentId, maxResults);

                if (string.IsNullOrWhiteSpace(query))
                {
                    _logger.LogWarning(" Empty query provided");
                    return new List<RetrievedChunk>();
                }

                if (pdfContentId.HasValue)
                {
                    var dbChunkCount = await _dbContext.pdfChunks
                        .Where(c => c.PdfContentId == pdfContentId.Value)
                        .CountAsync();

                    _logger.LogInformation(" Database check: Found {ChunkCount} chunks for PDF {PdfId}",
                        dbChunkCount, pdfContentId.Value);

                    if (dbChunkCount == 0)
                    {
                        _logger.LogWarning(" No chunks found in database for PDF {PdfId}. PDF may not be processed yet.",
                            pdfContentId.Value);
                        return new List<RetrievedChunk>();
                    }

                    var chunksWithVectors = await _dbContext.pdfChunks
                        .Where(c => c.PdfContentId == pdfContentId.Value && !string.IsNullOrEmpty(c.PineconeVectorId))
                        .CountAsync();

                    _logger.LogInformation(" Vector check: {VectorCount} out of {TotalCount} chunks have Pinecone vectors",
                        chunksWithVectors, dbChunkCount);

                    if (chunksWithVectors == 0)
                    {
                        _logger.LogWarning(" No chunks have Pinecone vector IDs for PDF {PdfId}. Vectors may not be stored yet.",
                            pdfContentId.Value);
                        return new List<RetrievedChunk>();
                    }
                }

                _logger.LogInformation(" Creating embedding for query using Cohere");
                var embeddingResult = await _embeddingService.CreateEmbeddingAsync(query);

                if (!embeddingResult.Success || embeddingResult.Embedding == null)
                {
                    _logger.LogError("❌ Failed to create embedding for query: '{Query}'. Error: {Error}",
                        query, embeddingResult.ErrorMessage);
                    return new List<RetrievedChunk>();
                }

                _logger.LogInformation(" Successfully created embedding with {Dimensions} dimensions",
                    embeddingResult.Embedding.Length);

                Dictionary<string, object>? filter = null;
                if (pdfContentId.HasValue)
                {
                    filter = new Dictionary<string, object>
                    {
                        ["pdfContentId"] = pdfContentId.Value.ToString()
                    };
                    _logger.LogInformation(" Applied PDF filter for: {PdfId}", pdfContentId.Value);
                }
                else
                {
                    _logger.LogInformation(" No PDF filter applied - searching across all documents");
                }

                _logger.LogInformation(" Querying Pinecone with topK: {TopK}", maxResults);
                var pineconeResult = await _pineconeService.QueryByVectorAsync(
                    embeddingResult.Embedding,
                    topK: maxResults,
                    filter: filter,
                    includeMetadata: true
                );

                if (!pineconeResult.Success)
                {
                    _logger.LogError(" Pinecone query failed for query: '{Query}'. Error: {Error}",
                        query, pineconeResult.ErrorMessage);
                    return new List<RetrievedChunk>();
                }

                _logger.LogInformation(" Pinecone returned {MatchCount} matches", pineconeResult.Matches.Count);

                if (!pineconeResult.Matches.Any())
                {
                    _logger.LogWarning(" No similar chunks found in Pinecone for query: '{Query}'", query);

                    try
                    {
                        var indexStats = await _pineconeService.GetIndexStatsAsync();
                        _logger.LogInformation(" Pinecone index stats - Total vectors: {TotalVectors}, Index fullness: {Fullness}%",
                            indexStats.TotalVectorCount, indexStats.IndexFullness * 100);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not retrieve index stats");
                    }

                    return new List<RetrievedChunk>();
                }

                var matchDetails = pineconeResult.Matches.Select(m => new { Id = m.Id, Score = m.Score }).ToList();
                _logger.LogInformation(" Pinecone matches with scores: {Matches}",
                    string.Join(", ", matchDetails.Select(m => $"{m.Id}:{m.Score:F3}")));

                var chunkIds = new List<Guid>();
                var invalidIds = new List<string>();

                foreach (var match in pineconeResult.Matches)
                {
                    if (Guid.TryParse(match.Id, out var chunkId))
                    {
                        chunkIds.Add(chunkId);
                    }
                    else
                    {
                        invalidIds.Add(match.Id);
                        _logger.LogWarning(" Invalid chunk ID format in Pinecone match: {Id}", match.Id);
                    }
                }

                if (invalidIds.Any())
                {
                    _logger.LogWarning(" Found {Count} invalid chunk IDs: {InvalidIds}",
                        invalidIds.Count, string.Join(", ", invalidIds));
                }

                _logger.LogInformation("Searching database for {ChunkIdCount} chunk IDs", chunkIds.Count);

                var chunks = await _dbContext.pdfChunks
                    .Where(c => chunkIds.Contains(c.Id))
                    .ToListAsync();

                _logger.LogInformation(" Retrieved {DbChunkCount} chunks from database out of {RequestedCount}",
                    chunks.Count, chunkIds.Count);

                if (chunks.Count != chunkIds.Count)
                {
                    var foundIds = chunks.Select(c => c.Id).ToHashSet();
                    var missingIds = chunkIds.Where(id => !foundIds.Contains(id)).ToList();
                    _logger.LogWarning(" Missing {MissingCount} chunks from database: {MissingIds}",
                        missingIds.Count, string.Join(", ", missingIds));
                }

                var retrievedChunks = new List<RetrievedChunk>();
                var unmatchedPineconeIds = new List<string>();

                foreach (var match in pineconeResult.Matches)
                {
                    var chunk = chunks.FirstOrDefault(c => c.Id.ToString() == match.Id);
                    if (chunk != null)
                    {
                        retrievedChunks.Add(new RetrievedChunk
                        {
                            ChunkId = chunk.Id,
                            Content = chunk.TextContent,
                            SimilarityScore = match.Score,
                            PageNumbers = chunk.PageNumbers,
                            ChunkIndex = chunk.ChunkIndex,
                            PdfContentId = chunk.PdfContentId
                        });

                        _logger.LogDebug(" Matched chunk {ChunkId} with score {Score:F3}",
                            chunk.Id, match.Score);
                    }
                    else
                    {
                        unmatchedPineconeIds.Add(match.Id);
                        _logger.LogWarning(" Chunk not found in database for Pinecone match ID: {Id} (Score: {Score:F3})",
                            match.Id, match.Score);
                    }
                }

                if (unmatchedPineconeIds.Any())
                {
                    _logger.LogWarning("{UnmatchedCount} Pinecone matches could not be found in database: {UnmatchedIds}",
                        unmatchedPineconeIds.Count, string.Join(", ", unmatchedPineconeIds));
                }

                var sortedChunks = retrievedChunks.OrderByDescending(c => c.SimilarityScore).ToList();

                _logger.LogInformation(" Successfully found {ChunkCount} similar chunks with scores: {Scores}",
                    sortedChunks.Count, string.Join(", ", sortedChunks.Take(3).Select(c => c.SimilarityScore.ToString("F3"))));

                foreach (var chunk in sortedChunks.Take(3))
                {
                    _logger.LogDebug(" Chunk {ChunkId}: Score={Score:F3}, Page={Page}, Length={Length}, PDF={PdfId}",
                        chunk.ChunkId, chunk.SimilarityScore, chunk.PageNumbers, chunk.Content.Length, chunk.PdfContentId);
                }

                return sortedChunks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error searching similar chunks for query: '{Query}'", query);
                return new List<RetrievedChunk>();
            }
        }

        public async Task<AIQueryResponse> AskQuestionAsync(AIQueryRequest request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Processing AI question: {Question}", request.Question);

                var retrievedChunks = await SearchSimilarChunksAsync(
                    request.Question,
                    request.PdfContentId,
                    request.MaxChunks);

                if (!retrievedChunks.Any())
                {
                    return new AIQueryResponse
                    {
                        Success = false,
                        ErrorMessage = "No relevant content found for your question",
                        ProcessingTime = stopwatch.Elapsed
                    };
                }

                var filteredChunks = retrievedChunks
                    .Where(c => c.SimilarityScore >= request.SimilarityThreshold)
                    .ToList();

                if (!filteredChunks.Any())
                {
                    return new AIQueryResponse
                    {
                        Success = false,
                        ErrorMessage = $"No content found with similarity above {request.SimilarityThreshold:P0}",
                        ProcessingTime = stopwatch.Elapsed
                    };
                }

                _logger.LogInformation("Found {TotalChunks} relevant chunks, using {FilteredChunks} chunks above threshold {Threshold}",
                    retrievedChunks.Count, filteredChunks.Count, request.SimilarityThreshold);

                var geminiAnswer = await _geminiService.GenerateAnswerAsync(request.Question, filteredChunks);

                stopwatch.Stop();

                return new AIQueryResponse
                {
                    Success = true,
                    Answer = geminiAnswer,
                    RetrievedChunks = filteredChunks,
                    ProcessingTime = stopwatch.Elapsed,
                    Metadata = new AIQueryMetadata
                    {
                        TotalChunksFound = retrievedChunks.Count,
                        ChunksUsed = filteredChunks.Count,
                        HighestSimilarity = filteredChunks.FirstOrDefault()?.SimilarityScore ?? 0f,
                        SearchMethod = "Pinecone Vector Search + Gemini AI"
                    }
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing AI question with Gemini: {Question}", request.Question);

                return new AIQueryResponse
                {
                    Success = false,
                    ErrorMessage = $"Error processing question: {ex.Message}",
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        public async Task<AIQueryResponse> SummarizePdfAsync(PdfSummaryRequest request)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Generating summary for PDF: {PdfId} using Gemini", request.PdfContentId);

                var maxChunks = request.Length switch
                {
                    SummaryLength.Short => Math.Min(request.MaxChunks, 3),
                    SummaryLength.Medium => Math.Min(request.MaxChunks, 8),
                    SummaryLength.Long => request.MaxChunks,
                    _ => 5
                };

                var chunks = await _dbContext.pdfChunks
                    .Where(c => c.PdfContentId == request.PdfContentId)
                    .OrderBy(c => c.ChunkIndex)
                    .Take(maxChunks)
                    .ToListAsync();

                if (!chunks.Any())
                {
                    return new AIQueryResponse
                    {
                        Success = false,
                        ErrorMessage = "No content found for this PDF",
                        ProcessingTime = stopwatch.Elapsed
                    };
                }

                var retrievedChunks = chunks.Select(c => new RetrievedChunk
                {
                    ChunkId = c.Id,
                    Content = c.TextContent,
                    SimilarityScore = 1.0f,
                    PageNumbers = c.PageNumbers,
                    ChunkIndex = c.ChunkIndex,
                    PdfContentId = c.PdfContentId
                }).ToList();

                var summaryPrompt = BuildSummaryPrompt(retrievedChunks, request.Type, request.Length);
                var geminiSummary = await _geminiService.GenerateTextAsync(summaryPrompt);

                stopwatch.Stop();

                return new AIQueryResponse
                {
                    Success = true,
                    Answer = geminiSummary,
                    RetrievedChunks = retrievedChunks,
                    ProcessingTime = stopwatch.Elapsed,
                    Metadata = new AIQueryMetadata
                    {
                        TotalChunksFound = chunks.Count,
                        ChunksUsed = chunks.Count,
                        HighestSimilarity = 1.0f,
                        SearchMethod = "Sequential Document Reading + Gemini AI"
                    }
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error generating PDF summary with Gemini: {PdfId}", request.PdfContentId);

                return new AIQueryResponse
                {
                    Success = false,
                    ErrorMessage = $"Error generating summary: {ex.Message}",
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        private string BuildSummaryPrompt(List<RetrievedChunk> chunks, SummaryType type, SummaryLength length)
        {
            var prompt = new StringBuilder();

            var summaryInstruction = type switch
            {
                SummaryType.Overview => "Create a comprehensive overview of the document",
                SummaryType.KeyPoints => "Extract and list the key points from the document",
                SummaryType.Detailed => "Provide a detailed summary of the document",
                SummaryType.Abstract => "Create an abstract summary of the document",
                _ => "Summarize the document"
            };

            var lengthInstruction = length switch
            {
                SummaryLength.Short => "Keep the summary concise and brief (2-3 paragraphs)",
                SummaryLength.Medium => "Provide a medium-length summary (4-6 paragraphs)",
                SummaryLength.Long => "Create a comprehensive, detailed summary",
                _ => "Provide an appropriate length summary"
            };

            prompt.AppendLine($"You are a helpful AI assistant. {summaryInstruction}.");
            prompt.AppendLine($"{lengthInstruction}.");
            prompt.AppendLine("Base your summary only on the provided document content.");
            prompt.AppendLine();

            prompt.AppendLine("DOCUMENT CONTENT:");
            prompt.AppendLine("=" + new string('=', 50));

            foreach (var chunk in chunks)
            {
                prompt.AppendLine($"\n[Page {chunk.PageNumbers}]");
                prompt.AppendLine(chunk.Content);
                prompt.AppendLine();
            }

            prompt.AppendLine("=" + new string('=', 50));
            prompt.AppendLine();
            prompt.AppendLine("Please provide the summary now:");

            return prompt.ToString();
        }
    }
}
