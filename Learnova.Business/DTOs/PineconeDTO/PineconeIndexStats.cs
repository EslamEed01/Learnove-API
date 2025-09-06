using System.Text.Json.Serialization;

namespace Learnova.Business.DTOs.PineconeDTO
{
    public class PineconeIndexStats
    {
        [JsonPropertyName("dimension")]
        public int Dimension { get; set; }

        [JsonPropertyName("indexFullness")]
        public double IndexFullness { get; set; }

        [JsonPropertyName("totalVectorCount")]
        public long TotalVectorCount { get; set; }


    }
}