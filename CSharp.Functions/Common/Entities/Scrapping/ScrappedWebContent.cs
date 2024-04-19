using Newtonsoft.Json;

namespace Common.Entities
{
    public class ScrappedWebContent
    {
        [JsonProperty("id")] public string? Id { get; set; }
        public string? Url { get; set; }
        public string? HtmlContent { get; set; }
    }
}
