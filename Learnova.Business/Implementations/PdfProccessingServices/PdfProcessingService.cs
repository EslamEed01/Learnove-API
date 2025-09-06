using Amazon.S3;
using Amazon.S3.Model;
using Learnova.Business.DTOs.PdfProcessingDTO;
using Learnova.Business.DTOs.PineconeDTO;
using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Learnova.Business.Services.Interfaces.PdfProccessingServices;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Learnova.Business.Implementations.PdfProccessingServices
{
    public class PdfProcessingService : IPdfProcessingService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly LearnoveDbContext _dbContext;
        private readonly TextChunker _textChunker;
        private readonly IEmbeddingService _embeddingService;
        private readonly ILogger<PdfProcessingService> _logger;
        private readonly IPineconeService _pineconeService;

        public PdfProcessingService(
            IAmazonS3 s3Client,
            LearnoveDbContext dbContext,
            TextChunker textChunker,
            IEmbeddingService embeddingService,
            ILogger<PdfProcessingService> logger,
            IPineconeService pineconeService)
        {
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _textChunker = textChunker ?? throw new ArgumentNullException(nameof(textChunker));
            _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pineconeService = pineconeService;
        }

        public async Task<PdfProcessingResult> ProcessPdfAsync(
            Guid pdfContentId,
            string s3Bucket,
            string s3Key,
            PdfProcessingOptions? options = null)
        {
            options ??= new PdfProcessingOptions();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("Starting PDF processing for {PdfContentId} from s3://{Bucket}/{Key}",
                    pdfContentId, s3Bucket, s3Key);

                var pdfContent = await _dbContext.pdfContents
                    .FirstOrDefaultAsync(p => p.Id == pdfContentId);

                if (pdfContent == null)
                {
                    throw new ArgumentException($"PDF content with ID {pdfContentId} not found");
                }
                var pdfStream = await DownloadPdfFromS3Async(s3Bucket, s3Key);
                var fileSizeBytes = pdfStream.Length;

                var (textPages, metadata) = await ExtractPdfContentAsync(pdfStream, options.ExtractMetadata);

                if (options.ExtractMetadata && metadata != null)
                {
                    await UpdatePdfMetadataAsync(pdfContentId, metadata);
                }

                var chunks = _textChunker.SplitIntoChunks(
                    textPages,
                    pdfContentId,
                    options.ChunkSize,
                    options.OverlapSize);

                await SaveChunksInBatchesAsync(chunks, options.BatchSize);
                _logger.LogInformation("Creating embeddings for {ChunkCount} chunks using Cohere API", chunks.Count);
                var embeddings = await CreateEmbeddingsForChunksAsync(chunks, options.BatchSize);

                await StoreEmbeddingsInPineconeAsync(chunks, embeddings);

                stopwatch.Stop();

                var result = new PdfProcessingResult
                {
                    Success = true,
                    TotalPages = textPages.Count,
                    TotalChunks = chunks.Count,
                    ProcessingTime = stopwatch.Elapsed,
                    FileSizeBytes = fileSizeBytes,
                    Metadata = metadata
                };

                _logger.LogInformation("Successfully processed PDF {PdfContentId}. Pages: {Pages}, Chunks: {Chunks}, Time: {Time}ms",
                    pdfContentId, result.TotalPages, result.TotalChunks, result.ProcessingTime.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Failed to process PDF {PdfContentId}: {Error}", pdfContentId, ex.Message);

                return new PdfProcessingResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    ProcessingTime = stopwatch.Elapsed
                };
            }
        }

        private async Task<Dictionary<Guid, float[]>> CreateEmbeddingsForChunksAsync(List<PdfChunk> chunks, int batchSize)
        {
            _logger.LogInformation("Creating embeddings for {TotalChunks} chunks in batches of {BatchSize}",
                chunks.Count, batchSize);

            var embeddings = new Dictionary<Guid, float[]>();
            var embeddingTasks = new List<Task>();
            var semaphore = new SemaphoreSlim(5);

            for (int i = 0; i < chunks.Count; i += batchSize)
            {
                var batch = chunks.Skip(i).Take(batchSize).ToList();
                embeddingTasks.Add(ProcessEmbeddingBatchAsync(batch, embeddings, semaphore, i / batchSize + 1));
            }

            await Task.WhenAll(embeddingTasks);

            _logger.LogInformation("Successfully created embeddings for {Count} chunks", embeddings.Count);
            return embeddings;
        }

        private async Task ProcessEmbeddingBatchAsync(
            List<PdfChunk> batch,
            Dictionary<Guid, float[]> embeddings,
            SemaphoreSlim semaphore,
            int batchNumber)
        {
            await semaphore.WaitAsync();

            try
            {
                _logger.LogDebug("Processing embedding batch {BatchNumber} with {ChunkCount} chunks",
                    batchNumber, batch.Count);

                var texts = batch.Select(c => c.TextContent).ToList();
                var batchResult = await _embeddingService.CreateEmbeddingsAsync(texts);

                if (batchResult.Success)
                {
                    for (int i = 0; i < batch.Count && i < batchResult.Results.Count; i++)
                    {
                        if (batchResult.Results[i].Success)
                        {
                            lock (embeddings)
                            {
                                embeddings[batch[i].Id] = batchResult.Results[i].Embedding;
                            }

                            _logger.LogDebug("Created embedding for chunk {ChunkIndex} (dimension: {Dimension})",
                                batch[i].ChunkIndex, batchResult.Results[i].Embedding.Length);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to create embedding for chunk {ChunkIndex}: {Error}",
                                batch[i].ChunkIndex, batchResult.Results[i].ErrorMessage);
                        }
                    }
                }
                else
                {
                    _logger.LogError("Failed to create embeddings for batch {BatchNumber}: {Errors}",
                        batchNumber, string.Join(", ", batchResult.Errors));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing embedding batch {BatchNumber}", batchNumber);
            }
            finally
            {
                semaphore.Release();
            }
        }
        public async Task<bool> DeletePdfChunksAsync(Guid pdfContentId)
        {
            try
            {
                _logger.LogInformation("Deleting PDF chunks for {PdfContentId}", pdfContentId);

                var chunks = await _dbContext.pdfChunks
                    .Where(c => c.PdfContentId == pdfContentId)
                    .ToListAsync();

                if (chunks.Any())
                {
                    _dbContext.pdfChunks.RemoveRange(chunks);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation("Deleted {Count} chunks for PDF {PdfContentId}",
                        chunks.Count, pdfContentId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete PDF chunks for {PdfContentId}", pdfContentId);
                return false;
            }
        }

        public async Task<PdfProcessingSummary> GetProcessingSummaryAsync(Guid pdfContentId)
        {
            var chunks = await _dbContext.pdfChunks
                .Where(c => c.PdfContentId == pdfContentId)
                .Select(c => new { c.PageNumbers, c.CreatedAt })
                .ToListAsync();

            var uniquePages = chunks
                .SelectMany(c => c.PageNumbers.Split(','))
                .Where(p => int.TryParse(p.Trim(), out _))
                .Distinct()
                .Count();

            return new PdfProcessingSummary
            {
                TotalChunks = chunks.Count,
                TotalPages = uniquePages,
                ProcessedAt = chunks.FirstOrDefault()?.CreatedAt ?? DateTime.UtcNow
            };
        }
        private async Task<MemoryStream> DownloadPdfFromS3Async(string bucket, string key)
        {
            const int maxRetries = 3;
            var delay = TimeSpan.FromSeconds(1);

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var request = new GetObjectRequest { BucketName = bucket, Key = key };
                    using var response = await _s3Client.GetObjectAsync(request);

                    var memoryStream = new MemoryStream();
                    await response.ResponseStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    _logger.LogDebug("Successfully downloaded PDF from S3 on attempt {Attempt}", attempt);
                    return memoryStream;
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    _logger.LogWarning("Failed to download PDF from S3 on attempt {Attempt}: {Error}. Retrying...",
                        attempt, ex.Message);
                    await Task.Delay(delay);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * 2);
                }
            }

            throw new InvalidOperationException($"Failed to download PDF from S3 after {maxRetries} attempts");
        }

        private async Task<(List<(int pageNumber, string text)>, PdfMetadata?)> ExtractPdfContentAsync(
            Stream pdfStream,
            bool extractMetadata)
        {
            return await Task.Run(() =>
            {
                var textPages = new List<(int pageNumber, string text)>();
                PdfMetadata? metadata = null;

                using var pdf = PdfDocument.Open(pdfStream);

                if (extractMetadata)
                {
                    metadata = new PdfMetadata
                    {
                        Title = pdf.Information?.Title,
                        Author = pdf.Information?.Author,
                        Subject = pdf.Information?.Subject,
                        Creator = pdf.Information?.Creator,
                        PageCount = pdf.NumberOfPages
                    };
                }

                foreach (Page page in pdf.GetPages())
                {
                    try
                    {
                        var pageText = ExtractPageText(page);
                        textPages.Add((page.Number, pageText));

                        _logger.LogDebug("Extracted text from page {PageNumber}: {CharCount} characters",
                            page.Number, pageText.Length);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("Failed to extract text from page {PageNumber}: {Error}",
                            page.Number, ex.Message);
                        textPages.Add((page.Number, string.Empty));
                    }
                }

                return (textPages, metadata);
            });
        }
        private string ExtractPageText(Page page)
        {
            var text = page.Text;

            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var lines = text.Split('\n');
            var cleanedLines = new List<string>();

            foreach (var line in lines)
            {
                var cleanLine = line.Trim();
                if (!string.IsNullOrEmpty(cleanLine))
                {
                    cleanedLines.Add(cleanLine);
                }
                else if (cleanedLines.Count > 0 && !cleanedLines.Last().EndsWith("\n\n"))
                {
                    if (cleanedLines.Count > 0)
                        cleanedLines[cleanedLines.Count - 1] += "\n\n";
                }
            }

            return string.Join(" ", cleanedLines);
        }

        private async Task UpdatePdfMetadataAsync(Guid pdfContentId, PdfMetadata metadata)
        {
            var pdfContent = await _dbContext.pdfContents
                .FirstOrDefaultAsync(p => p.Id == pdfContentId);

            if (pdfContent != null)
            {
                pdfContent.TotalPages = metadata.PageCount;
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task SaveChunksInBatchesAsync(List<PdfChunk> chunks, int batchSize)
        {
            _logger.LogInformation("Saving {TotalChunks} chunks (text only) in batches of {BatchSize}",
                chunks.Count, batchSize);

            for (int i = 0; i < chunks.Count; i += batchSize)
            {
                var batch = chunks.Skip(i).Take(batchSize).ToList();

                try
                {
                    _dbContext.pdfChunks.AddRange(batch);
                    await _dbContext.SaveChangesAsync();

                    _logger.LogDebug("Saved batch {BatchNumber}: {BatchSize} chunks (text only)",
                        i / batchSize + 1, batch.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save batch {BatchNumber}", i / batchSize + 1);
                    throw;
                }
            }
        }

        private async Task StoreEmbeddingsInPineconeAsync(List<PdfChunk> chunks, Dictionary<Guid, float[]> embeddings)
        {
            try
            {
                _logger.LogInformation("Storing {ChunkCount} embeddings in Pinecone", embeddings.Count);

                var vectors = chunks
                    .Where(c => embeddings.ContainsKey(c.Id))
                    .Select(chunk => new PineconeVector
                    {
                        Id = chunk.Id.ToString(),
                        Values = embeddings[chunk.Id],
                        Metadata = new Dictionary<string, object>
                        {
                            ["pdfContentId"] = chunk.PdfContentId.ToString(),
                            ["chunkIndex"] = chunk.ChunkIndex,
                            ["pageNumbers"] = chunk.PageNumbers,
                            ["textContent"] = chunk.TextContent.Substring(0, Math.Min(chunk.TextContent.Length, 1000)), // Truncate for metadata
                            ["createdAt"] = chunk.CreatedAt.ToString("O")
                        }
                    })
                    .ToList();

                if (vectors.Any())
                {
                    var pineconeResult = await _pineconeService.UpsertVectorsAsync(vectors);

                    if (pineconeResult.Success)
                    {
                        _logger.LogInformation("Successfully stored {Count} vectors in Pinecone", pineconeResult.UpsertedCount);

                        await UpdatePineconeVectorIdsAsync(chunks, embeddings);
                    }
                    else
                    {
                        _logger.LogError("Failed to store vectors in Pinecone: {Error}", pineconeResult.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing embeddings in Pinecone");
            }
        }
        private async Task UpdatePineconeVectorIdsAsync(List<PdfChunk> chunks, Dictionary<Guid, float[]> embeddings)
        {
            try
            {
                var chunksToUpdate = chunks.Where(c => embeddings.ContainsKey(c.Id)).ToList();

                foreach (var chunk in chunksToUpdate)
                {
                    chunk.PineconeVectorId = chunk.Id.ToString();
                }

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Updated {Count} chunks with Pinecone vector IDs", chunksToUpdate.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Pinecone vector IDs in database");
            }
        }
    }
}