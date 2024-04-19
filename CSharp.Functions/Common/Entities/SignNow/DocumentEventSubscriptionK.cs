using Newtonsoft.Json;

namespace Common.Entities
{
    public class DocumentEventSubscriptionK
    {
        [JsonProperty("event")] public string EventName { get; set; }
        [JsonProperty("entity_id")] public string DocumentId { get; set; }
        [JsonProperty("action")] public string Action { get; set; } = "callback";
        [JsonProperty("attributes")] public EventAttributesK Attributes { get; set; }
    }
}
