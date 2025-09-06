using Learnova.Business.DTOs.AiDTO;
using Learnova.Business.DTOs.GeminiDTO;
using Learnova.Business.Services.Interfaces.PdfProccessingInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Learnova.Business.Implementations.PdfProccessingServices
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiOptions _options;
        private readonly ILogger<GeminiService> _logger;

        public GeminiService(
            HttpClient httpClient,
            IOptions<GeminiOptions> options,
            ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = _options.RequestTimeout;
        }

        public async Task<string> GenerateAnswerAsync(string question, List<RetrievedChunk> chunks)
        {
            try
            {
                _logger.LogInformation("Generating answer using Gemini for question: {Question} with {ChunkCount} chunks",
                    question, chunks.Count);

                var prompt = BuildPromptFromChunks(question, chunks);
                return await GenerateTextAsync(prompt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating answer with Gemini");
                throw;
            }
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(prompt))
                {
                    throw new ArgumentException("Prompt cannot be null or empty", nameof(prompt));
                }

                var request = new GeminiRequest
                {
                    Contents = new List<GeminiContent>
                    {
                        new GeminiContent
                        {
                            Parts = new List<GeminiPart>
                            {
                                new GeminiPart { Text = prompt }
                            }
                        }
                    },
                    GenerationConfig = new GeminiGenerationConfig
                    {
                        Temperature = _options.Temperature,
                        TopK = _options.TopK,
                        TopP = _options.TopP,
                        MaxOutputTokens = _options.MaxTokens
                    }
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                };

                var json = JsonSerializer.Serialize(request, jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"/v1beta/models/{_options.DefaultModel}:generateContent?key={_options.ApiKey}";

                _logger.LogDebug("Making Gemini API call to: {BaseUrl}{Url}", _options.BaseUrl, url);

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API call failed with status {StatusCode}: {ErrorContent}. Full URL: {FullUrl}",
                        response.StatusCode, errorContent, $"{_options.BaseUrl}{url}");
                    throw new HttpRequestException($"Gemini API call failed: {response.StatusCode} - {errorContent}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(responseJson))
                {
                    throw new InvalidOperationException("Empty response from Gemini API");
                }

                _logger.LogDebug("Gemini API response: {Response}", responseJson);

                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson, jsonOptions);

                if (geminiResponse?.Candidates?.Any() == true &&
                    geminiResponse.Candidates[0].Content?.Parts?.Any() == true)
                {
                    var generatedText = geminiResponse.Candidates[0].Content.Parts[0].Text;

                    if (string.IsNullOrWhiteSpace(generatedText))
                    {
                        throw new InvalidOperationException("Gemini API returned empty text");
                    }

                    _logger.LogInformation("Successfully generated text using Gemini. Length: {Length} characters",
                        generatedText.Length);

                    return generatedText;
                }

                throw new InvalidOperationException("No valid response from Gemini API");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                throw;
            }
        }


        private string BuildPromptFromChunks(string question, List<RetrievedChunk> chunks)
        {
            var prompt = new StringBuilder();

            prompt.AppendLine("You are a helpful AI assistant. Answer the user's question based on the provided document content.");
            prompt.AppendLine("Please provide a comprehensive and accurate answer using only the information from the given context.");
            prompt.AppendLine("If the context doesn't contain enough information to answer the question, please say so.");
            prompt.AppendLine();

            prompt.AppendLine("CONTEXT FROM DOCUMENT:");
            prompt.AppendLine("=" + new string('=', 50));

            for (int i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                prompt.AppendLine($"\n[Chunk {i + 1} - Page {chunk.PageNumbers} - Relevance: {chunk.SimilarityScore:P1}]");
                prompt.AppendLine(chunk.Content);
                prompt.AppendLine();
            }
            prompt.AppendLine("=" + new string('=', 50));
            prompt.AppendLine();
            prompt.AppendLine($"QUESTION: {question}");
            prompt.AppendLine();
            prompt.AppendLine("ANSWER:");

            return prompt.ToString();
        }
    }
}