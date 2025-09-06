using Learnova.Business.DTOs.PineconeDTO;
using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Learnova.Business.Implementations.PdfProccessingServices
{
    public class PineconeService : IPineconeService
    {
        private readonly HttpClient _httpClient;
        private readonly PineconeOptions _options;
        private readonly ILogger<PineconeService> _logger;

        public PineconeService(
            HttpClient httpClient,
            IOptions<PineconeOptions> options,
            ILogger<PineconeService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Api-Key", _options.ApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = _options.RequestTimeout;
        }

        public async Task<PineconeUpsertResult> UpsertVectorsAsync(IEnumerable<PineconeVector> vectors, string? indexName = null)
        {
            var targetIndex = indexName ?? _options.IndexName;

            try
            {
                var vectorList = vectors.ToList();
                _logger.LogInformation("Upserting {Count} vectors to Pinecone index {Index}", vectorList.Count, targetIndex);

                foreach (var vector in vectorList)
                {
                    if (vector.Values.Length != _options.Dimension)
                    {
                        throw new ArgumentException($"Vector {vector.Id} has dimension {vector.Values.Length}, expected {_options.Dimension}");
                    }
                }

                var request = new
                {
                    vectors = vectorList.Select(v => new
                    {
                        id = v.Id,
                        values = v.Values,
                        metadata = CleanMetadata(v.Metadata)
                    }),
                    @namespace = ""
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                var json = JsonSerializer.Serialize(request, jsonOptions);
                _logger.LogDebug("Upsert request JSON: {Json}", json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/vectors/upsert", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Pinecone upsert failed with {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}). Error: {errorContent}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseJson);

                return new PineconeUpsertResult
                {
                    Success = true,
                    UpsertedCount = result.TryGetProperty("upsertedCount", out var count)
                        ? count.GetInt32() : vectorList.Count,
                    ProcessedIds = vectorList.Select(v => v.Id).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting vectors to Pinecone index {Index}", targetIndex);
                return new PineconeUpsertResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        private static Dictionary<string, object>? CleanMetadata(Dictionary<string, object>? metadata)
        {
            if (metadata == null) return null;

            var cleanedMetadata = new Dictionary<string, object>();

            foreach (var kvp in metadata)
            {
                object value;
                if (kvp.Value is bool b)
                    value = b;
                else if (kvp.Value is string s)
                    value = s;
                else if (kvp.Value is int i)
                    value = i;
                else if (kvp.Value is long l)
                    value = l;
                else if (kvp.Value is float f)
                    value = f;
                else if (kvp.Value is double d)
                    value = d;
                else if (kvp.Value is DateTime dt)
                    value = dt.ToString("O");
                else
                    value = kvp.Value?.ToString() ?? "";

                cleanedMetadata[kvp.Key] = value;
            }

            return cleanedMetadata;
        }

        public async Task<PineconeQueryResult> QueryByVectorAsync(
            float[] vector,
            int topK = 10,
            object? filter = null,
            bool includeMetadata = true,
            string? indexName = null)
        {
            var targetIndex = indexName ?? _options.IndexName;

            try
            {
                var request = new
                {
                    vector = vector,
                    topK = topK,
                    includeMetadata = includeMetadata,
                    includeValues = false,
                    filter = filter,
                    @namespace = ""
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"/query", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseJson);

                var matches = new List<PineconeMatch>();

                if (result.TryGetProperty("matches", out var matchesArray))
                {
                    foreach (var match in matchesArray.EnumerateArray())
                    {
                        var pineconeMatch = new PineconeMatch
                        {
                            Id = match.GetProperty("id").GetString() ?? "",
                            Score = match.GetProperty("score").GetSingle()
                        };

                        if (match.TryGetProperty("metadata", out var metadata))
                        {
                            pineconeMatch.Metadata = metadata;
                        }

                        matches.Add(pineconeMatch);
                    }
                }

                return new PineconeQueryResult
                {
                    Success = true,
                    Matches = matches
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying Pinecone index {Index}", targetIndex);
                return new PineconeQueryResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Matches = new List<PineconeMatch>()
                };
            }
        }

        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/describe_index_stats");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public async Task<PineconeIndexStats> GetIndexStatsAsync(string? indexName = null)
        {
            try
            {
                var response = await _httpClient.GetAsync("/describe_index_stats");
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PineconeIndexStats>(responseJson);

                return result ?? new PineconeIndexStats();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting index stats for {Index}", indexName ?? _options.IndexName);
                throw;
            }
        }



    }
}
