using Learnova.Business.DTOs.EmbeddingDTO;
using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Learnova.Business.Implementations.PdfProccessingServices
{
    public class CohereEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;
        private readonly CohereOptions _options;
        private readonly IEmbeddingStatsService _statsService;
        private readonly ILogger<CohereEmbeddingService> _logger;

        public CohereEmbeddingService(
            HttpClient httpClient,
            IOptions<CohereOptions> options,
            IEmbeddingStatsService statsService,
            ILogger<CohereEmbeddingService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _statsService = statsService;
            _logger = logger;

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = _options.RequestTimeout;
        }

        public async Task<EmbeddingResult> CreateEmbeddingAsync(string text, EmbeddingOptions? options = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var model = options?.Model ?? _options.DefaultModel;

            try
            {
                var embedding = await CallCohereApiAsync(text, model);
                stopwatch.Stop();

                var result = new EmbeddingResult
                {
                    Embedding = embedding,
                    TokenCount = EstimateTokenCount(text),
                    Model = model,
                    IsFromCache = false,
                    RequestDurationMs = stopwatch.Elapsed.TotalMilliseconds,
                    Success = true
                };

                _statsService.RecordRequest(result.TokenCount, false, stopwatch.Elapsed.TotalMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error creating embedding for text of length {Length}", text.Length);

                return new EmbeddingResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    RequestDurationMs = stopwatch.Elapsed.TotalMilliseconds,
                    Model = model
                };
            }
        }

        public async Task<BatchEmbeddingResult> CreateEmbeddingsAsync(IEnumerable<string> texts, EmbeddingOptions? options = null)
        {
            var textList = texts.ToList();
            var results = new List<EmbeddingResult>();
            var batchSize = options?.BatchSize ?? _options.BatchSize;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var batches = textList
                .Select((text, index) => new { text, index })
                .GroupBy(x => x.index / batchSize)
                .Select(g => g.Select(x => x.text).ToList())
                .ToList();

            foreach (var batch in batches)
            {
                try
                {
                    var batchResults = await ProcessBatchAsync(batch, options);
                    results.AddRange(batchResults);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing batch of {Count} texts", batch.Count);

                    foreach (var text in batch)
                    {
                        results.Add(new EmbeddingResult
                        {
                            Success = false,
                            ErrorMessage = ex.Message,
                            Model = options?.Model ?? _options.DefaultModel
                        });
                    }
                }
            }

            stopwatch.Stop();

            return new BatchEmbeddingResult
            {
                Results = results,
                TotalProcessed = textList.Count,
                SuccessCount = results.Count(r => r.Success),
                FailureCount = results.Count(r => !r.Success),
                TotalDurationMs = stopwatch.Elapsed.TotalMilliseconds,
                Errors = results.Where(r => !r.Success).Select(r => r.ErrorMessage ?? "Unknown error").ToList()
            };
        }

        private async Task<List<EmbeddingResult>> ProcessBatchAsync(List<string> texts, EmbeddingOptions? options)
        {
            var tasks = texts.Select(text => CreateEmbeddingAsync(text, options));
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        private async Task<float[]> CallCohereApiAsync(string text, string model)
        {
            var requestBody = new
            {
                texts = new[] { text },
                model = model,
                input_type = "search_document"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/embed", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseJson);

            if (responseObj.TryGetProperty("embeddings", out var embeddingsElement) &&
                embeddingsElement.ValueKind == JsonValueKind.Array &&
                embeddingsElement.GetArrayLength() > 0)
            {
                var firstEmbedding = embeddingsElement[0];
                if (firstEmbedding.ValueKind == JsonValueKind.Array)
                {
                    var embedding = new float[firstEmbedding.GetArrayLength()];
                    for (int i = 0; i < embedding.Length; i++)
                    {
                        embedding[i] = firstEmbedding[i].GetSingle();
                    }
                    return embedding;
                }
            }

            throw new InvalidOperationException("Invalid response format from Cohere API");
        }

        public async Task<EmbeddingServiceStats> GetStatsAsync()
        {
            return await Task.FromResult(_statsService.GetStatsSnapshot());
        }

        public async Task<bool> ValidateApiKeyAsync()
        {
            try
            {
                var testText = "test";
                var result = await CreateEmbeddingAsync(testText);
                return result.Success;
            }
            catch
            {
                return false;
            }
        }

        private static int EstimateTokenCount(string text)
        {
            return (int)Math.Ceiling(text.Length / 4.0);
        }
    }
}