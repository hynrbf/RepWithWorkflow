using Newtonsoft.Json;

namespace Common.Entities
{
    public class CalendlyEventTypeCollectionResultK
    {
        [JsonProperty("collection")] public IEnumerable<CalendlyEventTypeK> Collection { get; set; } = new List<CalendlyEventTypeK>();
    }
}
