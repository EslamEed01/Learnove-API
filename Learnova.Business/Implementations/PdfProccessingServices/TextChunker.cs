using Learnova.Domain.Entities;
using System.Text;
using System.Text.RegularExpressions;

namespace Learnova.Business.Implementations.PdfProccessingServices
{
    public class TextChunker
    {
        public List<PdfChunk> SplitIntoChunks(
            List<(int pageNumber, string text)> textPages,
            Guid pdfContentId,
            int chunkSize = 1000,
            int overlapSize = 100)
        {
            var chunks = new List<PdfChunk>();
            var chunkIndex = 0;

            foreach (var (pageNumber, text) in textPages)
            {
                if (string.IsNullOrWhiteSpace(text)) continue;

                var cleanText = CleanText(text);

                var pageChunks = CreateSemanticChunks(cleanText, chunkSize, overlapSize);

                foreach (var chunkText in pageChunks)
                {
                    if (string.IsNullOrWhiteSpace(chunkText)) continue;

                    chunks.Add(new PdfChunk
                    {
                        Id = Guid.NewGuid(),
                        PdfContentId = pdfContentId,
                        ChunkIndex = chunkIndex++,
                        PageNumbers = pageNumber.ToString(),
                        TextContent = chunkText.Trim(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            return chunks;
        }

        private List<string> CreateSemanticChunks(string text, int chunkSize, int overlapSize)
        {
            var chunks = new List<string>();

            var paragraphs = text.Split(new[] { "\n\n", "\r\n\r\n" },
                StringSplitOptions.RemoveEmptyEntries);

            var currentChunk = new StringBuilder();

            foreach (var paragraph in paragraphs)
            {
                if (currentChunk.Length + paragraph.Length > chunkSize && currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    var overlapText = GetLastWords(currentChunk.ToString(), overlapSize);
                    currentChunk.Clear();
                    if (!string.IsNullOrEmpty(overlapText))
                    {
                        currentChunk.Append(overlapText).Append(" ");
                    }
                }

                currentChunk.Append(paragraph).Append("\n\n");
                if (paragraph.Length > chunkSize)
                {
                    chunks.AddRange(SplitLongParagraph(paragraph, chunkSize, overlapSize));
                    currentChunk.Clear();
                }
            }
            if (currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
            }

            return chunks;
        }

        private List<string> SplitLongParagraph(string paragraph, int chunkSize, int overlapSize)
        {
            var chunks = new List<string>();

            var sentences = paragraph.Split(new[] { ". ", "! ", "? " },
                StringSplitOptions.RemoveEmptyEntries);

            var currentChunk = new StringBuilder();

            foreach (var sentence in sentences)
            {
                var fullSentence = sentence + ". ";

                if (currentChunk.Length + fullSentence.Length > chunkSize && currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    var overlapText = GetLastWords(currentChunk.ToString(), overlapSize);
                    currentChunk.Clear();
                    if (!string.IsNullOrEmpty(overlapText))
                    {
                        currentChunk.Append(overlapText).Append(" ");
                    }
                }

                currentChunk.Append(fullSentence);
            }

            if (currentChunk.Length > 0)
            {
                chunks.Add(currentChunk.ToString());
            }

            return chunks;
        }

        private string GetLastWords(string text, int maxLength)
        {
            if (text.Length <= maxLength) return text;

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();

            for (int i = words.Length - 1; i >= 0; i--)
            {
                if (result.Length + words[i].Length + 1 > maxLength) break;

                if (result.Length > 0)
                    result.Insert(0, " ");
                result.Insert(0, words[i]);
            }

            return result.ToString();
        }

        private string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            text = Regex.Replace(text, @"\s+", " ");

            text = Regex.Replace(text, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

            text = text.Replace("\r\n", "\n").Replace("\r", "\n");

            return text.Trim();
        }
    }
}
