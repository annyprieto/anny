using System.Text.Json.Serialization;

namespace Frankfurter.AnnyPriet.Entidades
{
    public class TasaDeCambioJson
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; } = null!;

        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; } = null!;
    }
}
